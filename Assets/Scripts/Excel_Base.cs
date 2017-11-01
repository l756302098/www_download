using UnityEngine;
using System.Collections;
using System;

public class Excel_Base {

    private char[] SplitCharacters = { '\n', '\r', '\t', '=' };

    protected string[] ReadExcelRow(string excelRow)
    {
        string[] results = excelRow.Split(SplitCharacters, System.StringSplitOptions.RemoveEmptyEntries);
        return results;
    }

    protected virtual string[] ReadExcel(string content, bool bReadInLine = false)
    {
        if ("" != content)
        {
            if (bReadInLine)
            {
                string[] results = content.Split('\n');
                return results;
            }
            else
            {
                string[] results = content.Split(SplitCharacters, System.StringSplitOptions.RemoveEmptyEntries);
                return results;
            }
        }
        return null;
    }

    public virtual void LoadData(string filename)
    {

    }
}
