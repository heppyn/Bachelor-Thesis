using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThermostatControler
{
    public interface IData
    {
        void SaveUsers(List<User> users);
        List<User> LoadUsers();

        void SaveSchedules(List<Schedule> schedules);
        List<Schedule> LoadSchedules();

        void SaveProfiles(List<Profile> profiles);
        List<Profile> LoadProfiles();


        void SaveHeaters(Heaters heaters);
        Heaters LoadHeaters();
    }
}
