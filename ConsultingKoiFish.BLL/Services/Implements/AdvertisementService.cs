using AutoMapper;
using ConsultingKoiFish.BLL.Services.Interfaces;
using ConsultingKoiFish.DAL.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsultingKoiFish.BLL.Services.Implements
{
	public class AdvertisementService : IAdvertisementService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IMapper _mapper;

		public AdvertisementService(IUnitOfWork unitOfWork, IMapper mapper)
        {
			this._unitOfWork = unitOfWork;
			this._mapper = mapper;
		}
    }
}
