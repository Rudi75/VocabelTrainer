using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Assets
{
    public class VocabelDb
    {
        private static string fileName = "vocabels.txt";
        private static VocabelDb instance_;

        private VocabelDb()
        {

        }

        public static VocabelDb instance
        {
            get
            {
                if (instance_ == null)
                {
                    instance_ = new VocabelDb();
                }
                return instance_;
            }
        }

        private List<Vocabel> vocabels_ = new List<Vocabel>();
        public List<Vocabel> vocabels
        {
            get
            {
                return vocabels_;
            }
        }

        public void fill()
        {
            if (File.Exists(fileName))
            {
                vocabels_.Clear();
                string[] content = File.ReadAllLines(fileName,Encoding.UTF8);
                foreach (string line in content)
                {
                    string[] parts = line.Split(';');

                    int level = Int32.Parse(parts[3]);
                    DateTime lastTrainingTime = DateTime.Parse(parts[0]);
                    if (level >= 5 && lastTrainingTime.AddDays(7) < DateTime.Now)
                    {
                        level--;
                    }

                    vocabels_.Add(new Vocabel(parts[1], parts[2], level, lastTrainingTime));
                }
            }
            else
            {
                File.Create(fileName);
            }


        }

        public void writeToFile()
        {
            StringBuilder builder = new StringBuilder();

            foreach (Vocabel vocabel in vocabels_)
            {
                builder.AppendLine(vocabel.lastTrainingTime + ";" + getText(vocabel.english) + ";" + getText(vocabel.german) + ";" + vocabel.level);
            }

            File.WriteAllText(fileName, builder.ToString(),Encoding.UTF8);
        }

        private string getText(List<string> english)
        {
            string result = "";
            for (int i = 0; i < english.Count; i++)
            {
                if (i != 0)
                {
                    result += "/";
                }
                result += english[i];
            }
            return result;
        }

    }
}
