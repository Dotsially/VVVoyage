using TestProject.Class;

namespace TestProject.Tests
{
    public class DatabaseTests
    {
        [Fact]
        private async Task IsDataBaseFileMade()
        {
            // Arrange
            SQLAppDatabase db = new SQLAppDatabase();

            // Act
            await db.Init();

            // Assert
            string path = Path.Combine(Microsoft.VisualBasic.FileIO.FileSystem.CurrentDirectory, "vvvoyage.db");
            Assert.True(File.Exists(path));
        }

        [Fact]
        private async Task CanAddLandmark()
        {
            // Arrange
            SQLAppDatabase db = new SQLAppDatabase();
            await db.Init();

            Sight sight = new Sight("test", new Location(), "cool");

            // Act
            bool result = await db.AddLandmarkAsync(sight);

            // Assert
            Assert.True(result);
        }
    }
}
