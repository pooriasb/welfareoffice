using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BehzistiMaskan.Core.Dtos;
using BehzistiMaskan.Core.Models;
using BehzistiMaskan.Core.Utility;
using BehzistiMaskan.Models;

namespace BehzistiMaskan.Controllers.Api
{
    public class SecurityQuestionController : ApiController
    {
        //Get new Code
        public IHttpActionResult GetSecurityQuestion()
        {
            var question = new SecurityCaptcha().GenerateQuestion();

            return Ok(new SecurityQuestionDto {Question = question.Question, QuestionId = question.Id});
        }

        
    }
}
