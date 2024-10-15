using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsultingKoiFish.DAL.Entities
{
	public partial class Zodiac
	{
		public int Id { get; set; }
		public string ZodiacName { get; set; }

		public virtual ICollection<UserZodiac> UserZodiacs { get; set; }
		public virtual ICollection<KoiBreedZodiac> KoiBreedZodiacs { get; set; } = new List<KoiBreedZodiac>();
		public virtual ICollection<PondZodiac> PondZodiacs { get; set; } = new List<PondZodiac>();
	}
}
