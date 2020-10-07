using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NewsPublish.Test.Areas
{
    [Route("[area]/[controller]/[action]")]
    public class BaseController : Controller
    {
    }
}
