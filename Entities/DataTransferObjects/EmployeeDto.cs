using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Entities.DataTransferObjects
{
    public class EmployeeDto
    {
       
        public Guid Id { get; set; }
       
        public string Name { get; set; }
       
        public int Age { get; set; }
      
    }
}
