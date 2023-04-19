using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace TaskManagementSystem.Models
{
    public class TaskDataModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [DisplayName("Task Description")]
        public string TaskDescription { get; set; }

        [DisplayName("Task Date")]
        public DateTime TaskDate { get; set; }

        public Guid CreatedUser { get; set; }
        public DateTime CreatedTime { get; set; }
    }
}
