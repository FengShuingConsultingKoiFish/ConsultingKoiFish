using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsultingKoiFish.DAL.Entities
{
	public partial class KoiBreedZodiac
	{
		public int Id { get; set; }
		public int KoiBreedId { get; set; }
		public int ZodiacId { get; set; }

		public virtual KoiBreed KoiBreed { get; set; }
		public virtual Zodiac Zodiac { get; set; }
	}
}
