using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsultingKoiFish.DAL.Entities
{
	public partial class KoiDetail
	{
        public int Id { get; set; }
        public int UserPondId { get; set; }
        public int KoiBreedId { get; set; }

        public virtual UserPond UserPond { get; set; }
        public virtual KoiBreed KoiBreed { get; set; }
    }
}
