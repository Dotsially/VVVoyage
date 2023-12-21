using TestProject.Class;

namespace TestProject.Tests
{
    public class DatabaseTests
    {
        [Fact]
        private async Task CanAddLandmark()
        {
            // Arrange
            SQLAppDatabase db = new SQLAppDatabase();
            await db.Init();

            Sight sight = new Sight("test", new Location(), "cool", "");

            // Act
            bool result = await db.AddLandmarkAsync(sight);

            // Assert
            Assert.True(result);
        }
    }
}
