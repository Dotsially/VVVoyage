using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VVVoyage.Subsystems.Navigation
{
    interface INavigator
    {
        Task<MapUpdate?> UpdateMapAsync();
        void CancelMapUpdate();
    }
}
