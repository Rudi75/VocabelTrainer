using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Assets
{
    public class Vocabel
    {
        public List<String> english = new List<string>();
        public List<String> german = new List<string>();
        public int level;
        public DateTime lastTrainingTime;

        public Vocabel(String english, String german)
        {
            fill(this.english, english);
            fill(this.german, german);

            this.level = 0;
            this.lastTrainingTime = DateTime.MinValue;
        }

        private void fill(List<string> wordList, string wordString)
        {
            Regex reg = new Regex("^\\s*$");
            if(reg.IsMatch(wordString))
            {
                return;
            }
            reg = new Regex("/|" + Regex.Escape("\\") + "|" + Regex.Escape("|"));
            string[] parts = reg.Split(wordString);
            foreach (string part_ in parts)
            {
                string part = handleSpecialChars(part_);
                wordList.Add(part);
            }
        }

        private string handleSpecialChars(string part)
        {
            Regex reg = new Regex(Regex.Escape("`") + "|" + Regex.Escape("´"));
            part = reg.Replace(part, "'");
            part = part.Replace("\r", "");
            part = part.Replace("\n", "");
            reg = new Regex("\\s*$");
            part = reg.Replace(part, "");
            reg = new Regex("^\\s*");
            part = reg.Replace(part,"");
            return part;
        }

        public Vocabel(String english, String german, int level, DateTime lastTrainingTime)
        {
            fill(this.english, english);
            fill(this.german, german);
            this.level = level;
            this.lastTrainingTime = lastTrainingTime;
        }

        public void wrong()
        {
            lastTrainingTime = DateTime.Now;
            level--;
            if (level < -3)
            {
                level = -3;
            }
        }

        public void right()
        {
            lastTrainingTime = DateTime.Now;
            level++;
            if (level > 3)
                level = 3;
        }
    }
}
