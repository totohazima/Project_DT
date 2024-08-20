using System.Collections;
using System.Collections.Generic;
using GDBA;
using NPOI.SS.UserModel;
using StatusHelper;
using UnityEditor;
using UnityEngine;

public class WeaponInfoImporter : CustomExcelDataImportBase
{
    public WeaponInfoImporter(string _OutPath) : base(_OutPath) { }

    public override void ImporteExcel(string _excelName, ISheet sheet)
    {
        EquipmentInfoTable InfoTable = ExcelDataImporter.LoadOrCreateAsset<EquipmentInfoTable>("Assets/Resources/ExcelData/GameInfoData/", _excelName, HideFlags.None);
        InfoTable.table = new EquipmentInfoTable.Data[sheet.LastRowNum];

        string temp = null;
        string riseStatusKey = null;
        EquipmentInfoTable.Data data = null;

        for (int i = 1; i <= sheet.LastRowNum; i++)
        {
            IRow row = sheet.GetRow(i);
            ICell cell = null;
            InfoTable.table[i - 1] = new EquipmentInfoTable.Data();
            data = InfoTable.table[i - 1];

            cell = row.GetCell(0); data.spriteCode = (cell == null ? "" : cell.StringCellValue);
            cell = row.GetCell(1); data.code = (cell == null ? "" : cell.StringCellValue);
            cell = row.GetCell(2); data.name = (cell == null ? "" : cell.StringCellValue);
            cell = row.GetCell(3); data.grade = (EquipmentItem.EquipmentGrade)System.Enum.Parse(typeof(EquipmentItem.EquipmentGrade), (cell == null ? "" : cell.StringCellValue));
            cell = row.GetCell(4); data.baseStatus.statusType = (ESTATUS)System.Enum.Parse(typeof(ESTATUS), (cell == null ? "" : cell.StringCellValue));
            cell = row.GetCell(5); data.baseStatus.riseType = (ERISE_TYPE)System.Enum.Parse(typeof(ERISE_TYPE), (cell == null ? "" : cell.StringCellValue));
            cell = row.GetCell(6); data.baseStatus.baseValue = (cell == null ? 0 : cell.NumericCellValue);
            cell = row.GetCell(7); data.baseStatus.riseValue = (cell == null ? 0 : cell.NumericCellValue);
            cell = row.GetCell(8); data.maxLevel = (int)(cell == null ? 0 : cell.NumericCellValue);
        }
        EditorUtility.SetDirty(InfoTable);
    }
}
