using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VVVoyage.Models;

namespace VVVoyage.Subsystems.Navigation
{
    interface INavigator
    {
        Task<MapUpdate?> UpdateMapAsync(Sight landmarkToReach);
        void CancelMapUpdate();
    }
}
