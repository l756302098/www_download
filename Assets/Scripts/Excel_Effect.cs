using System.Collections.Generic;

public class Excel_Effect : Excel_Base
{
    static Excel_Effect mInstance;
    private static List<Data_Effect> datasList = new List<Data_Effect>(); 

    public static Excel_Effect Instance()
    {
        if (null == mInstance)
        {
            mInstance = new Excel_Effect();
        }
        return mInstance;
    }

    public override void LoadData(string content)
    {
        if (datasList.Count != 0) return;
        string[] results = ReadExcel(content, true);
        int nLen = results.Length;

        for (int i = 1; i < nLen; i++)
        {
            string[] datas = ReadExcelRow(results[i]);
            if (datas.Length == 0)
            {
                continue;
            }
            Data_Effect data = new Data_Effect();
            int istart = 0;
            data.id = System.Convert.ToInt32(datas[istart]);
            istart++;
            data.name = datas[istart];
            istart++;
            //UnityEngine.Debug.Log("data:" + datas[istart]+ " istart:" + istart);
            string[] array = datas[istart].Split(',');
            List<KeyValuePair<string, float>> effect = null;
            for (int j = 0; j < array.Length; j++)
            {
                string[] pairs = array[j].Split(':');
                if (effect == null) effect = new List<KeyValuePair<string, float>>();
                effect.Add(new KeyValuePair<string, float>(pairs[0], System.Convert.ToInt32(pairs[1])));
            }
            data.effect = effect;
            datasList.Add(data);
        }
    }

    public Data_Effect GetById(int id)
    {
        for (int i = 0; i < datasList.Count; i++)
        {
            if (datasList[i].id == id)
            {
                return datasList[i];
            }
        }
        return null;
    }


    public List<Data_Effect> GetList()
    {
        return datasList;
    }
}

