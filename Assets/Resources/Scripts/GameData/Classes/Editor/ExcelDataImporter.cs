using UnityEngine;
using UnityEditor;
using System.IO;
using NPOI.SS.UserModel;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace GDBA
{
    public class ExcelDataImporter : EditorWindow
    {
        private Vector2 curretScroll = Vector2.zero;

        System.Type classType = null;

        void OnGUI()
        {
            GUILayout.Label("makeing importer", EditorStyles.boldLabel);
            string Temp = EditorGUILayout.TextField("class name", className);
            if (Temp.Equals(className) == false)
                classType = System.Type.GetType(Temp + ", Assembly-CSharp");

            className = Temp;

            /**/
            if (GUILayout.Button("Create"))
            {
                classType = System.Type.GetType(className + ", Assembly-CSharp");
                if (classType == null && IgnoreClass(className) == false)
                {
                    CreateGameDataTableClass();
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();

                    EditorUtility.DisplayDialog("알림", "잠시후 Excel Data Import를 다시 시도하시오", "확인");
                    Close();
                    return;
                }
                if (NeedCreateClass(className, GetSheet(filePath)) == true && IgnoreClass(className) == false)
                {
                    if (EditorUtility.DisplayDialog("알림", "Excel Key가 갱신되어 Class를 새로 만들어야 됩니다.", "생성", "취소") == false)
                    {
                        Close();
                        return;
                    }
                    if (GetCustomExcelDataImport(className) != null)
                    {
                        if (EditorUtility.DisplayDialog("알림", "커스텀 Class 입니다. 직접 수정해야 됩니다.", "확인") == true)
                        {
                            Close();
                            return;
                        }
                    }

                    CreateGameDataTableClass();
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();

                    EditorUtility.DisplayDialog("알림", "잠시후 Excel Data Import를 다시 시도하시오", "확인");
                    Close();
                    return;
                }

                CustomExcelDataImportBase customExcelDataImport = GetCustomExcelDataImport(className);
                ImporteExcel(filePath, className, customExcelDataImport);

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                Close();
            }

            // selecting parameters
            //if (classType != null || string.IsNullOrEmpty(className) == true)
            if (string.IsNullOrEmpty(className) == true)
                return;

            EditorGUILayout.LabelField("parameter settings");
            curretScroll = EditorGUILayout.BeginScrollView(curretScroll);
            EditorGUILayout.BeginVertical("box");
            string lastCellName = string.Empty;
            foreach (ExcelRowParameter cell in typeList)
            {
                if (cell.isArray && lastCellName != null && cell.name.Equals(lastCellName))
                {
                    continue;
                }

                cell.isEnable = EditorGUILayout.BeginToggleGroup("enable", cell.isEnable);
                if (cell.isArray)
                {
                    EditorGUILayout.LabelField("---[array]---");
                }
                GUILayout.BeginHorizontal();
                cell.name = EditorGUILayout.TextField(cell.name);
                cell.type = (ValueType)EditorGUILayout.EnumPopup(cell.type, GUILayout.MaxWidth(100));
                EditorPrefs.SetInt(s_key_prefix + fileName + ".type." + cell.name, (int)cell.type);
                GUILayout.EndHorizontal();

                EditorGUILayout.EndToggleGroup();
                lastCellName = cell.name;
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndScrollView();
        }

        private enum ValueType
        {
            BOOL,
            STRING,
            INT,
            FLOAT,
            DOUBLE,
        }

        private bool outParam = false;
        private string outParamClassName = string.Empty;
        private string outParamFileName = string.Empty;

        private string filePath = string.Empty;
        private string fileName = string.Empty;

        private string className = string.Empty;

        private List<ExcelRowParameter> typeList = new List<ExcelRowParameter>();
        private List<ExcelSheetParameter> sheetList = new List<ExcelSheetParameter>();
        private string customImportName = string.Empty;
        private static string s_key_prefix = "terasurware.exel-importer-maker.";

        [MenuItem("Assets/[Excel Data Import]")]
        static void ExportExcelToAssetbundle()
        {
            foreach (Object obj in Selection.objects)
            {
                var window = ScriptableObject.CreateInstance<ExcelDataImporter>();
                window.filePath = AssetDatabase.GetAssetPath(obj);
                window.fileName = Path.GetFileNameWithoutExtension(window.filePath);

                using (FileStream stream = File.Open(window.filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    IWorkbook book = null;
                    if (Path.GetExtension(window.filePath) == ".xls")
                    {
                        book = new HSSFWorkbook(stream);
                    }
                    else
                    {
                        book = new XSSFWorkbook(stream);
                    }

                    for (int i = 0; i < book.NumberOfSheets; ++i)
                    {
                        ISheet s = book.GetSheetAt(i);
                        ExcelSheetParameter sht = new ExcelSheetParameter();
                        sht.sheetName = s.SheetName;
                        sht.isEnable = EditorPrefs.GetBool(s_key_prefix + window.fileName + ".sheet." + sht.sheetName, true);
                        window.sheetList.Add(sht);
                    }

                    ISheet sheet = book.GetSheetAt(0);
                    // fileName
                    //window.className = EditorPrefs.GetString(s_key_prefix + window.fileName + ".className", "Entity_" + sheet.SheetName);
                    window.className = window.fileName;
                    window.classType = System.Type.GetType(window.className + ", Assembly-CSharp");

                    window.outParamClassName = EditorPrefs.GetString(s_key_prefix + window.fileName + ".paramClassName", "Entity_Param_" + sheet.SheetName);
                    window.outParamFileName = EditorPrefs.GetString(s_key_prefix + window.fileName + ".paramFileName", "Entity_Param_" + sheet.SheetName);

                    IRow titleRow = sheet.GetRow(0);
                    IRow dataRow = sheet.GetRow(1);
                    for (int i = 0; i < titleRow.LastCellNum; i++)
                    {
                        ExcelRowParameter lastParser = null;
                        ExcelRowParameter parser = new ExcelRowParameter();
                        parser.name = titleRow.GetCell(i).StringCellValue;
                        parser.isArray = parser.name.Contains("[]");
                        if (parser.isArray)
                        {
                            parser.name = parser.name.Remove(parser.name.LastIndexOf("[]"));
                        }

                        ICell cell = dataRow.GetCell(i);

                        // array support
                        if (window.typeList.Count > 0)
                        {
                            lastParser = window.typeList[window.typeList.Count - 1];
                            if (lastParser.isArray && parser.isArray && lastParser.name.Equals(parser.name))
                            {
                                // trailing array items must be the same as the top type
                                parser.isEnable = lastParser.isEnable;
                                parser.type = lastParser.type;
                                lastParser.nextArrayItem = parser;
                                window.typeList.Add(parser);
                                continue;
                            }
                        }

                        if (cell.CellType != CellType.Unknown && cell.CellType != CellType.Blank)
                        {
                            parser.isEnable = true;

                            try
                            {
                                if (EditorPrefs.HasKey(s_key_prefix + window.fileName + ".type." + parser.name))
                                {
                                    parser.type = (ValueType)EditorPrefs.GetInt(s_key_prefix + window.fileName + ".type." + parser.name);
                                }
                                else
                                {
                                    string sampling = cell.StringCellValue;
                                    parser.type = ValueType.STRING;
                                }
                            }
                            catch { }
                            try
                            {
                                if (EditorPrefs.HasKey(s_key_prefix + window.fileName + ".type." + parser.name))
                                {
                                    parser.type = (ValueType)EditorPrefs.GetInt(s_key_prefix + window.fileName + ".type." + parser.name);
                                }
                                else
                                {
                                    double sampling = cell.NumericCellValue;
                                    parser.type = ValueType.DOUBLE;
                                }
                            }
                            catch { }
                            try
                            {
                                if (EditorPrefs.HasKey(s_key_prefix + window.fileName + ".type." + parser.name))
                                {
                                    parser.type = (ValueType)EditorPrefs.GetInt(s_key_prefix + window.fileName + ".type." + parser.name);
                                }
                                else
                                {
                                    bool sampling = cell.BooleanCellValue;
                                    parser.type = ValueType.BOOL;
                                }
                            }
                            catch { }
                        }

                        window.typeList.Add(parser);
                    }

                    window.Show();
                }
            }
        }

        ////////////////////////////////////////////////////////////////////
        //
        ISheet GetSheet(string assetPath)
        {
            ISheet sheet = null;
            using (FileStream stream = File.Open(assetPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                IWorkbook book = null;
                if (Path.GetExtension(assetPath) == ".xls")
                {
                    book = new HSSFWorkbook(stream);
                }
                else
                {
                    book = new XSSFWorkbook(stream);
                }

                // get sheet
                sheet = book.GetSheetAt(0);
                if (sheet == null)
                {
                    Debug.LogError("[QuestData] sheet not found:" + "Sheet1");
                    return null;
                }
            }

            return sheet;
        }

        bool NeedCreateClass(string assetName, ISheet sheet)
        {
            if (classType == null)
                return true;

            System.Type dataType = System.Type.GetType(assetName + "+Data, Assembly-CSharp");

            MemberInfo[] memberInfos = null;
            string key = null;
            IRow keyRow = sheet.GetRow(0);
            ICell keyCell = null;
            for (int i = 0; i < keyRow.LastCellNum; i++)
            {
                try
                {
                    keyCell = keyRow.GetCell(i);
                    key = keyCell.StringCellValue;

                    memberInfos = dataType.GetMember(key);
                    if (memberInfos == null || memberInfos.Length <= 0)
                        return true;
                }
                catch (System.Exception e)
                {
                    return true;
                }
            }

            return false;
        }

        void ImporteExcel(string assetPath, string assetName, CustomExcelDataImportBase importer = null)
        {
            System.Type classType = System.Type.GetType(assetName + ", Assembly-CSharp");
            if (classType == null && importer == null)
            {
                Debug.LogError("not find class");
                return;
            }

            // Get sheet
            ISheet sheet = GetSheet(assetPath);
            if (sheet == null)
            {
                Debug.LogError("[QuestData] sheet not found:" + "Sheet1");
                return;
            }

            List<string> keys = new List<string>();
            IRow keyRow = sheet.GetRow(0);
            ICell keyCell = null;
            for (int i = 0; i < keyRow.LastCellNum; i++)
            {
                keyCell = keyRow.GetCell(i);
                keys.Add(keyCell.StringCellValue);
            }
            if (importer == null)
            {
                ImporteExcelBase(sheet, keys, assetName);
                Debug.Log("ImporteExcel " + assetName + " Done");
                return;
            }

            /**/
            importer.SetKey(keys);
            importer.ImporteExcel(fileName, sheet);
            Debug.Log("ImporteExcel " + assetName + " Done");
            //try
            //{
            //    importer.SetKey(keys);
            //    importer.ImporteExcel(fileName, sheet);
            //    Debug.Log("ImporteExcel " + assetName + " Done");
            //}
            //catch (System.Exception e)
            //{
            //    Debug.LogError("ImporteExcel Error" + e.ToString());
            //}
        }

        void ImporteExcelBase(ISheet sheet, List<string> keys, string assetName)
        {
            System.Type dataType = System.Type.GetType(assetName + "+Data, Assembly-CSharp");
            System.Array table = System.Array.CreateInstance(dataType, sheet.LastRowNum);

            // add infomation
            MemberInfo memberInfo = null;
            System.Type dataFieldType = null;
            for (int i = 1; i <= sheet.LastRowNum; i++)
            {
                IRow row = sheet.GetRow(i);
                ICell cell = null;

                object param = System.Activator.CreateInstance(dataType);
                for (int j = 0; j < keys.Count; j++)
                {
                    cell = row.GetCell(j);

                    memberInfo = param.GetType().GetMember(keys[j])[0];
                    dataFieldType = ((FieldInfo)memberInfo).FieldType;
                    if (dataFieldType.Equals(typeof(bool)))
                    {
                        SetValue(memberInfo, param, (bool)cell.BooleanCellValue);
                    }
                    if (dataFieldType.Equals(typeof(int)))
                    {
                        SetValue(memberInfo, param, (int)cell.NumericCellValue);
                    }
                    if (dataFieldType.Equals(typeof(float)))
                    {
                        SetValue(memberInfo, param, (float)cell.NumericCellValue);
                    }
                    if (dataFieldType.Equals(typeof(double)))
                    {
                        SetValue(memberInfo, param, (double)cell.NumericCellValue);
                    }
                    if (dataFieldType.Equals(typeof(string)))
                    {
                        if (cell.CellType == CellType.Numeric)
                            SetValue(memberInfo, param, cell.NumericCellValue.ToString());
                        else
                            SetValue(memberInfo, param, (string)cell.StringCellValue);
                    }
                }
                table.SetValue(param, i - 1);
            }

            var data = LoadOrCreateAsset("Assets/ExcelData/", assetName, classType);
            MemberInfo dataTable = data.GetType().GetMember("table")[0];
            SetValue(dataTable, data, table);

            EditorUtility.SetDirty(data);
        }

        public static T LoadAsset<T>(string exportPath, string assetName) where T : UnityEngine.ScriptableObject
        {
            return AssetDatabase.LoadAssetAtPath<T>(exportPath + assetName + ".asset");
        }

        public static ScriptableObject LoadOrCreateAsset(string exportPath, string assetName, System.Type type, HideFlags hideFlags = HideFlags.NotEditable)
        {
            string fullPath = exportPath + assetName + ".asset";
            ScriptableObject asset = (ScriptableObject)AssetDatabase.LoadAssetAtPath(fullPath, type);
            if (asset == null)
            {
                asset = ScriptableObject.CreateInstance(type);
                AssetDatabase.CreateAsset(asset, fullPath);
            }
            asset.hideFlags = hideFlags;

            return asset;
        }
        public static T LoadOrCreateAsset<T>(string exportPath, string assetName, HideFlags hideFlags = HideFlags.NotEditable) where T : UnityEngine.ScriptableObject
        {
            T asset = (T)LoadOrCreateAsset(exportPath, assetName, typeof(T), hideFlags);

            return asset;
        }
        //public static void SaveAsset(ScriptableObject asset)
        //{
        //    //AssetDatabase.ImportAsset("Assets/ExcelData/" + assetName + ".asset");
        //    EditorUtility.SetDirty(asset);
        //    AssetDatabase.SaveAssets();
        //    AssetDatabase.Refresh();
        //}

        void SetValue(MemberInfo memberInfo, object obj, object value)
        {
            if (memberInfo.MemberType != MemberTypes.Field)
            {
                Debug.LogError("not find memberInfo, if value is private ??");
                return;
            }
            ((FieldInfo)memberInfo).SetValue(obj, value);
        }

        void CreateGameDataTableClass()
        {
            string templateFilePath = "Assets/Script/GameDataBase/GameDataTableOutTemplate.txt";
            string gameDataTableTemplate = File.ReadAllText(templateFilePath);
            gameDataTableTemplate = gameDataTableTemplate.Replace("\r\n", "\n").Replace("\n", System.Environment.NewLine);

            StringBuilder builder = new StringBuilder();
            bool isInbetweenArray = false;
            foreach (ExcelRowParameter row in typeList)
            {
                if (row.isEnable)
                {
                    if (!row.isArray)
                    {
                        builder.AppendLine();
                        builder.AppendFormat("		public {0} {1};", row.type.ToString().ToLower(), row.name);
                    }
                    else
                    {
                        if (!isInbetweenArray)
                        {
                            builder.AppendLine();
                            builder.AppendFormat("		public {0}[] {1};", row.type.ToString().ToLower(), row.name);
                        }
                        isInbetweenArray = (row.nextArrayItem != null);
                    }
                }
            }

            gameDataTableTemplate = gameDataTableTemplate.Replace("$Types$", builder.ToString());
            gameDataTableTemplate = gameDataTableTemplate.Replace("$GameDataTable$", className);

            Directory.CreateDirectory("Assets/Script/GameDataBase/Classes/");
            File.WriteAllText("Assets/Script/GameDataBase/Classes/" + className + ".cs", gameDataTableTemplate);
        }

        ////////////////////////////////////////////////////////////////////
        // 커스텀을 사용하기 위함과 동시에 class 생성의 예외를 위함
        bool IgnoreClass(string _className)
        {
            switch (_className)
            {
                case "WeaponInfoTable":
                    return true;
                case "HelmetInfoTable":
                    return true;
                case "CharacterInfoTable":
                    return true;
            }
            return true;
        }

        // 커스텀으로 생성될 엑셀 지정 CustomExcelDataImportBase 생성
        CustomExcelDataImportBase GetCustomExcelDataImport(string _className)
        {
            string filePath = "Assets/Resources/ExcelData/GameInfoData/";
            switch (_className)
            {
                case "WeaponInfoTable":
                    return new WeaponInfoImporter(filePath);
                case "HelmetInfoTable":
                    return new HelmetInfoImporter(filePath);
                case "CharacterInfoTable":
                    return new CharacterInfoImporter(filePath);
            }

            return null;
        }

        private class ExcelSheetParameter
        {
            public string sheetName;
            public bool isEnable;
        }

        private class ExcelRowParameter
        {
            public ValueType type;
            public string name;
            public bool isEnable;
            public bool isArray;
            public ExcelRowParameter nextArrayItem;
        }
    }
}

