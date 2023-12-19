using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VVVoyage.Models;

namespace VVVoyage.Subsystems.Navigation
{
    public interface INavigator
    {
        Task<MapUpdate?> UpdateMapAsync(Sight landmarkToReach);
        Task<Location> GetUserLocationAsync(CancellationToken cancellationToken);
        void CancelMapUpdate();
    }
}
