using FawryPayIntegration.Dto;

namespace FawryPayIntegration.Contract
{
    public interface IGenerateFawrySignature
    {
        public string GenerateFawrySign(OnlineOrder order, Plan plan, string pathLang);
    }
}
