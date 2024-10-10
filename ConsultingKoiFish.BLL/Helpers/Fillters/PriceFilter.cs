using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsultingKoiFish.BLL.Helpers.Fillters
{
    public enum PriceFilter
    {
		Under500K = 1,       
		From500KTo1M = 2,     
		From1MTo2M = 3,      
		From2MTo3M = 4,      
		Above3M = 5,
	}

	public class PriceRangeHelper
	{
		public static (double min, double max) GetPriceRange(PriceFilter range)
		{
			switch (range)
			{
				case PriceFilter.Under500K:
					return (0, 499999);
				case PriceFilter.From500KTo1M:
					return (500000, 1000000);
				case PriceFilter.From1MTo2M:
					return (1000000, 2000000);
				case PriceFilter.From2MTo3M:
					return (2000000, 3000000);
				case PriceFilter.Above3M:
					return (3000000, double.MaxValue);
				default:
					throw new ArgumentOutOfRangeException(nameof(range), range, null);
			}
		}
	}
}
