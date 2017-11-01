using System.Collections.Generic;

public class Excel_EffectArray : Excel_Base
{
    static Excel_EffectArray mInstance;
    private static List<Data_EfffectArray> datasList = new List<Data_EfffectArray>();

    public static Excel_EffectArray Instance()
    {
        if (null == mInstance)
        {
            mInstance = new Excel_EffectArray();
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
            Data_EfffectArray data = new Data_EfffectArray();
            int istart = 0;
            data.id = System.Convert.ToInt32(datas[istart]);
            istart++;
            string[] array = datas[istart].Split(',');
            List<int> effects = null;
            for (int j = 0; j < array.Length; j++)
            {
                if (effects == null) {
                    effects = new List<int>();
                }
                effects.Add(System.Convert.ToInt32(array[j]));
            }
            data.effects = effects;
            datasList.Add(data);
        }
    }

    public Data_EfffectArray GetById(int id)
    {
        for (int i = 0; i < datasList.Count; i++)
        {
            if (datasList[i].id == id) {
                return datasList[i];
            }
        }
        return null;
    }
    System.Random r = new System.Random();
    public List<Data_EfffectArray> GetList() {
        return datasList;
    }

   public  Data_EfffectArray GetRandom()
    {
        int id = r.Next(0,datasList.Count);
        return datasList[id];
    }
}

