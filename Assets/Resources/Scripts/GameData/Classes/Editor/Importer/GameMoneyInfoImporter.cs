using GDBA;
using NPOI.SS.UserModel;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GameMoneyInfoImporter : CustomExcelDataImportBase
{
    public GameMoneyInfoImporter(string _OutPath) : base(_OutPath) { }

    public override void ImporteExcel(string _excelName, ISheet sheet)
    {
        GameMoneyInfoTable InfoTable = ExcelDataImporter.LoadOrCreateAsset<GameMoneyInfoTable>("Assets/Resources/ExcelData/GameInfoData/", _excelName, HideFlags.None);
        InfoTable.table = new GameMoneyInfoTable.Data[sheet.LastRowNum];

        string temp = null;
        string riseStatusKey = null;
        GameMoneyInfoTable.Data data = null;

        for (int i = 1; i <= sheet.LastRowNum; i++)
        {
            IRow row = sheet.GetRow(i);
            ICell cell = null;
            InfoTable.table[i - 1] = new GameMoneyInfoTable.Data();
            data = InfoTable.table[i - 1];

            cell = row.GetCell(0); data.num = (cell == null ? "" : cell.StringCellValue);
            cell = row.GetCell(1); data.gameMoneyCode = (cell == null ? "" : cell.StringCellValue);
            cell = row.GetCell(2); data.gameMoneyName = (cell == null ? "" : cell.StringCellValue);
            cell = row.GetCell(3); data.spriteCode = (cell == null ? "" : cell.StringCellValue);
        }
        EditorUtility.SetDirty(InfoTable);
    }
}
