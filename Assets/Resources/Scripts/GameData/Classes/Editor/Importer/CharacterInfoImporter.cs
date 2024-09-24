using GDBA;
using NPOI.SS.UserModel;
using StatusHelper;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CharacterInfoImporter : CustomExcelDataImportBase
{
    public CharacterInfoImporter(string _OutPath) : base(_OutPath) { }

    public override void ImporteExcel(string _excelName, ISheet sheet)
    {
        CharacterInfoTable InfoTable = ExcelDataImporter.LoadOrCreateAsset<CharacterInfoTable>("Assets/Resources/ExcelData/GameInfoData/", _excelName, HideFlags.None);
        InfoTable.table = new CharacterInfoTable.Data[sheet.LastRowNum];

        string temp = null;
        string riseStatusKey = null;
        CharacterInfoTable.Data data = null;

        for (int i = 1; i <= sheet.LastRowNum; i++)
        {
            IRow row = sheet.GetRow(i);
            ICell cell = null;
            InfoTable.table[i - 1] = new CharacterInfoTable.Data();
            data = InfoTable.table[i - 1];

            cell = row.GetCell(0); data.characterCode = (cell == null ? "" : cell.StringCellValue);
            cell = row.GetCell(1); data.characterName = (cell == null ? "" : cell.StringCellValue);
            cell = row.GetCell(2); data.characterClass = (cell == null ? "" : cell.StringCellValue);
            cell = row.GetCell(3); data.characterCostumeCode = (cell == null ? "" : cell.StringCellValue);
        }
        EditorUtility.SetDirty(InfoTable);
    }
}
