using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsultingKoiFish.DAL.Entities
{
	public partial class PackageImage
	{
        public int Id { get; set; }
		public int AdvertisementPackageId { get; set; }
        public int ImageId { get; set; }

		public virtual Image Image { get; set; }
		public virtual AdvertisementPackage AdvertisementPackage { get; set; }
    }
}
