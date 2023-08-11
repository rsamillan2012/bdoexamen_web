using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BDO.Examen.Web.Models
{
    public class json_result_list<T>
    {
        public IEnumerable<T> result { get; set; }
        public pagination_bind pagination { get; set; }
    }
}