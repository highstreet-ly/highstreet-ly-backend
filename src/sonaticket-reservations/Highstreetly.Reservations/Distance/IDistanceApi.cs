using System.Threading.Tasks;

namespace Highstreetly.Reservations.Distance
{
    public interface IDistanceApi
    {
        Task<DistanceModel> IsWithinRange(string origin, string destination);
    }

    // public partial class DistanceModel
    // {
    //     public static DistanceModel FromJson(string json) => JsonConvert.DeserializeObject<DistanceModel>(json, Converter.Settings);
    // }
}