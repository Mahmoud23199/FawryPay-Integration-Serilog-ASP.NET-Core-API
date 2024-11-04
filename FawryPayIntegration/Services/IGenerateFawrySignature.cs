using FawryPayIntegration.Dto;

namespace FawryPayIntegration.Services
{
    public interface IGenerateFawrySignature
    {
        public string GenerateFawrySign(OnlineOrder order, Plan plan, string pathLang);
    }
}
