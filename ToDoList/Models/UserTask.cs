using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ToDoList.Models
{
    public class UserTask
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime Deadline { get; set; }
        public int User { get; set; }
        public TaskStatus Status { get; set; }
    }
}
