using VVVoyage.Models;

namespace TestProject.Tests
{
    public class ModelsTest
    {
        //Sight.CS
        [Fact]
        private void Label_of_SightPin_should_be_entered_string()
        {
            Location l = new Location();
            var sut = new Sight("LabelString", "adress", l, "sightDescription");

            Assert.Equal("LabelString", sut.SightPin.Label);
        }

        [Fact]
        private void Address_of_SightPin_should_be_entered_string_StandardLabel()
        {
            Location l = new Location();
            var sut = new Sight("address", l, "sightDescription", "");

            Assert.Equal("address", sut.SightPin.Address);
        }

        [Fact]
        private void Address_of_SightPin_should_be_entered_string_PersonalLabel()
        {
            Location l = new Location();
            var sut = new Sight("LabelString", "address", l, "sightDescription");

            Assert.Equal("address", sut.SightPin.Address);
        }

        [Fact]
        private void Location_of_SightPin_should_be_entered_Location_StandardLabel()
        {
            Location l = new Location();
            var sut = new Sight("address", l, "sightDescription", "");

            Assert.Equal(l, sut.SightPin.Location);
        }

        [Fact]
        private void Location_of_SightPin_should_be_entered_Location_PersonalLabel()
        {
            Location l = new Location();
            var sut = new Sight("LabelString", "address", l, "sightDescription");

            Assert.Equal(l, sut.SightPin.Location);
        }

        [Fact]
        private void SightDesciption_of_SightPin_should_be_entered_SightDescpription_StandardLabel()
        {
            Location l = new Location();
            var sut = new Sight("address", l, "sightDescription", "");

            Assert.Equal("sightDescription", sut.SightDescription);
        }

        [Fact]
        private void SightDesciption_of_SightPin_should_be_entered_SightDescpription_PersonalLabel()
        {
            Location l = new Location();
            var sut = new Sight("LabelString", "address", l, "sightDescription");

            Assert.Equal("sightDescription", sut.SightDescription);
        }

        [Fact]
        private void Sight_Label_Location_SightDescription_StandardLabel()
        {
            Location l = new Location();
            var sut = new Sight("address", l, "sightDescription", "");

            Assert.Equal("address", sut.SightPin.Address);
            Assert.Equal(l, sut.SightPin.Location);
            Assert.Equal("sightDescription", sut.SightDescription);
        }

        [Fact]
        private void Sight_Label_Location_SightDescription_PersonalLabel()
        {
            Location l = new Location();
            var sut = new Sight("labelString","address", l, "sightDescription");

            Assert.Equal("labelString", sut.SightPin.Label);
            Assert.Equal("address", sut.SightPin.Address);
            Assert.Equal(l, sut.SightPin.Location);
            Assert.Equal("sightDescription", sut.SightDescription);
        }


        //Tour.CS
        [Fact]
        private void Tour_name_should_be_entered_string()
        {
            Location l = new Location();

            var one = new Sight("sightName", l, "sightDescription", "");

            var sut = new Tour("name", "description", one);

            Assert.Equal("name", sut.Name);
        }

        [Fact]
        private void Tour_descpription_should_be_entered_string()
        {
            Location l = new Location();

            var one = new Sight("sightName", l, "sightDescription", "");

            var sut = new Tour("name", "description", one);

            Assert.Equal("description", sut.Description);
        }

        [Fact]
        private void Tour_landmarks_should_be_entered_landmarks()
        {
            Location l = new Location();

            var one = new Sight("sightName", l, "sightDescription", "");
            var two = new Sight("nameSight", l, "descriptionSight", "");
            var three = new Sight("name", l, "description", "");

            Sight[] sightArray = { one, two, three };

            var sut = new Tour("tourName", "tourDescription", one, two, three);

            Assert.Equal(sightArray, sut.Landmarks);
        }

        [Fact]
        private void Tour_Name_Description_Landmarks()
        {
            Location l = new Location();

            var one = new Sight("sightName", l, "sightDescription", "");
            var two = new Sight("nameSight", l, "descriptionSight", "");
            var three = new Sight("name", l, "description", "");

            Sight[] sightArray = { one, two, three };

            var sut = new Tour("tourName", "tourDescription", one, two, three);

            Assert.Equal(sightArray, sut.Landmarks);
            Assert.Equal("tourName", sut.Name);
            Assert.Equal("tourDescription", sut.Description);
        }
    }
}
