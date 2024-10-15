using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsultingKoiFish.DAL.Entities
{
	public partial class PondZodiac
	{
		public int Id { get; set; }
		public int PondId { get; set; }
		public int ZodiacId { get; set; }

		public virtual Pond Pond { get; set; }
		public virtual Zodiac Zodiac { get; set; }
	}
}
