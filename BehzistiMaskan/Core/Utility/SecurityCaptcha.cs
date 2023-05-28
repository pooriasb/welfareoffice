using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BehzistiMaskan.Core.Models;
using BehzistiMaskan.Models;

namespace BehzistiMaskan.Core.Utility
{
    public class SecurityCaptcha
    {
        private readonly ApplicationDbContext _dbContext;

        public SecurityCaptcha()
        {
            _dbContext = new ApplicationDbContext();
        }

        public SecurityQuestion GenerateQuestion()
        {
            //Clean Old security question
            var oldQuestions = _dbContext.SecurityQuestions.Where(s => s.Expire < DateTime.Now);
            _dbContext.SecurityQuestions.RemoveRange(oldQuestions);
            _dbContext.SaveChanges();


            //generate new question
            var newQuestion = new SecurityQuestion();
            var rand = new Random();
            var x = rand.Next(9);
            x++;
            var y = rand.Next(9);
            y++;
            if (y > x)
            {
                var t = x;
                x = y;
                y = t;
            }

            var op = rand.Next(3);
            var operandText = "";
            var res = 0;
            switch (op)
            {

                case 1:
                    operandText = " منهای ";
                    res = x - y;
                    break;
                case 2:
                    operandText = " ضربدر ";
                    res = x * y;
                    break;
                case 0:
                default:
                    operandText = " به اضافه ";
                    res = x + y;
                    break;
            }

            newQuestion.Question = $"{ConvertNumToText(x)} {operandText} {ConvertNumToText(y)}";
            newQuestion.Answer = res.ToString();
            newQuestion.Expire = DateTime.Now + new TimeSpan(0, 5, 0);

            _dbContext.SecurityQuestions.Add(newQuestion);
            _dbContext.SaveChanges();

            return newQuestion;
        }

        public string ConvertNumToText(int num)
        {
            switch (num)
            {
                case 0: return "صفر";
                case 1: return "یک";
                case 2: return "دو";
                case 3: return "سه";
                case 4: return "چهار";
                case 5: return "پنج";
                case 6: return "شش";
                case 7: return "هفت";
                case 8: return "هشت";
                case 9: return "نه";
                default: return "";

            }
        }

        public bool IsValid(int questionId, string answer)
        {
            var questionInDb = _dbContext.SecurityQuestions.SingleOrDefault(q => q.Id == questionId);
            if (questionInDb == null) return false;

            return questionInDb.Answer == answer ? true : false;
        }
    }
}