using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Merino.Infrastructure.Entity
{
    public class BaseEntity
    {

        [Column("create_pgm_id")]
        [Required]
        public string CreatePgmId { get; set; } = test();

        [Column("create_user_id")]
        [Required]
        public int CreateUserId { get; set; }

        [Column("create_date")]
        [Required]
        public DateTime CreateDate { get; set; } = DateTime.Now;

        [Column("update_pgm_id")]
        [Required]
        public string UpdatePgmId { get; set; } = test();

        [Column("update_user_id")]
        [Required]
        public int UpdateUserId { get; set; }

        [Column("update_date")]
        [Required]
        public DateTime UpdateDate { get; set; } = DateTime.Now;


        private static string test()
        {
            StackFrame frame = null;
            int i = 1;
            while (true)
            {
                frame = new StackFrame(i);

                if (frame.GetMethod().MemberType == MemberTypes.Method)
                {
                    break;
                }
                i++;
            }
            
            string className = frame.GetMethod().ReflectedType != null ? frame.GetMethod().ReflectedType.FullName : "";
            string method = frame.GetMethod().Name;
            return className + "#" + method;
        }
    }
}
