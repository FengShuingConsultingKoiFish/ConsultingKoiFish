using ConsultingKoiFish.BLL.DTOs.AdvertisementPackageDTOs;
using ConsultingKoiFish.DAL.Entities;
using Newtonsoft.Json;

namespace ConsultingKoiFish.BLL.DTOs.PaymentDTOs
{
    public class PaymentViewDTO
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public long TransactionId { get; set; }
        public string? Content { get; set; }
        public double Amount { get; set; }
        public string CreatedDate { get; set; }
        public AdvertisementPackageViewDTO AdvertisementPackageViewDTO { get; set; }

        public PaymentViewDTO(Payment payment, AdvertisementPackageViewDTO advertisementPackageViewDTO)
        {
            Id = payment.Id;
            UserName = payment.User.UserName;
            TransactionId = payment.TransactionId;
            Content = payment.Content;
            Amount = GetPaymentAmount(payment, advertisementPackageViewDTO);
            CreatedDate = payment.CreatedDate.ToString("dd/MM/yyyy");
            if (!string.IsNullOrEmpty(payment.ClonePackage))
            {
                AdvertisementPackageViewDTO = JsonConvert.DeserializeObject<AdvertisementPackageViewDTO>(payment.ClonePackage);
            }
            else
            {
                AdvertisementPackageViewDTO = null;
            }
        }

        public double GetPaymentAmount(Payment payment, AdvertisementPackageViewDTO advertisementPackageViewDto)
        {
            var paymentAmount = payment.Amount;
            var adverPrice = advertisementPackageViewDto.Price;
            if (paymentAmount / adverPrice == 100)
            {
                return adverPrice;
            }

            return paymentAmount;
        }
    }
}
