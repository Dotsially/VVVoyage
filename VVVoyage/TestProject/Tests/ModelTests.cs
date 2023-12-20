using VVVoyage.Models;

namespace TestProject.Tests
{
    public class ModelsTest
    {
        [Fact]
        public void Label_of_SightPin_should_be_set_when_not_entered()
        {
            Location l = new Location();
            var sut = new Sight("adress", l, "sightDescription");

            Assert.Equal("Bezienswaardigheid", sut.SightPin.Label);
        }

        [Fact]
        public void Label_of_SightPin_should_be_entered_string()
        {
            Location l = new Location();
            var sut = new Sight("LabelString", "adress", l, "sightDescription");

            Assert.Equal("LabelString", sut.SightPin.Label);
        }

        [Fact]
        public void Address_of_SightPin_should_be_entered_string_StandardLabel()
        {
            Location l = new Location();
            var sut = new Sight("address", l, "sightDescription");

            Assert.Equal("address", sut.SightPin.Address);
        }

        [Fact]
        public void Address_of_SightPin_should_be_entered_string_PersonalLabel()
        {
            Location l = new Location();
            var sut = new Sight("LabelString", "address", l, "sightDescription");

            Assert.Equal("address", sut.SightPin.Address);
        }

        [Fact]
        public void Location_of_SightPin_should_be_entered_Location_StandardLabel()
        {
            Location l = new Location();
            var sut = new Sight("address", l, "sightDescription");

            Assert.Equal(l, sut.SightPin.Location);
        }

        [Fact]
        public void Location_of_SightPin_should_be_entered_Location_PersonalLabel()
        {
            Location l = new Location();
            var sut = new Sight("LabelString", "address", l, "sightDescription");

            Assert.Equal(l, sut.SightPin.Location);
        }

        [Fact]
        public void SightDesciption_of_SightPin_should_be_entered_SightDescpription_StandardLabel()
        {
            Location l = new Location();
            var sut = new Sight("address", l, "sightDescription");

            Assert.Equal("sightDescription", sut.SightDescription);
        }

        [Fact]
        public void SightDesciption_of_SightPin_should_be_entered_SightDescpription_PersonalLabel()
        {
            Location l = new Location();
            var sut = new Sight("LabelString", "address", l, "sightDescription");

            Assert.Equal("sightDescription", sut.SightDescription);
        }
    }
}
