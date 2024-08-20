using NPOI.SS.UserModel;
using System.Collections.Generic;

namespace GDBA
{                               
    public abstract class CustomExcelDataImportBase
    {
        protected string OutPath = null;
        protected List<string> keys = null;

        public CustomExcelDataImportBase(string _OutPath)
        {
            OutPath = _OutPath;
        }

        public void SetKey(List<string> _keys)
        {
            keys = _keys;
        }

        public abstract void ImporteExcel(string _excelName, ISheet sheet);
    }
}