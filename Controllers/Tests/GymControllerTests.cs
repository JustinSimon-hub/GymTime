using Xunit;
using Moq;
using GymTime.Controllers;
using GymTime.Models;
using GymTime.Models.Data_Transfer_Object;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using FluentAssertions;

namespace GymTesting
{
    public class GymControllerTests
    {
        private readonly Mock<IGymRepository> _mockRepo;
        private readonly GymController _controller;

        public GymControllerTests()
        {
            _mockRepo = new Mock<IGymRepository>();
            _controller = new GymController(_mockRepo.Object);

            // Setup session for authentication
            var httpContext = new DefaultHttpContext();
            httpContext.Session = new MockHttpSession();
            httpContext.Session.SetInt32("UserId", 1);
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
        }

        #region Index Tests

        [Fact]
        public void Index_ShouldReturnViewWithGymViewModel()
        {
            // Arrange
            var diets = new List<Diet>
            {
                new Diet { Id = 1, UserId = 1, FoodName = "Chicken", Proteins = 30, Fats = 3, Carbohydrates = 0, Calories = 165 }
            };
            var workouts = new List<Workout>
            {
                new Workout { Id = 1, UserId = 1, WorkoutName = "Squats", Reps = 10, Sets = 3, PersonalRecord = 225, Description = "Leg day" }
            };

            _mockRepo.Setup(r => r.GetDietsByUser(1)).Returns(diets);
            _mockRepo.Setup(r => r.GetWorkoutsByUser(1)).Returns(workouts);

            // Act
            var result = _controller.Index() as ViewResult;

            // Assert
            result.Should().NotBeNull();
            var model = result!.Model as GymViewModel;
            model.Should().NotBeNull();
            model!.Diets.Should().HaveCount(1);
            model.Workouts.Should().HaveCount(1);
            model.Diets.First().FoodName.Should().Be("Chicken");
            model.Workouts.First().WorkoutName.Should().Be("Squats");
        }

        [Fact]
        public void Index_ShouldCallRepositoryMethodsOnce()
        {
            // Arrange
            _mockRepo.Setup(r => r.GetDietsByUser(It.IsAny<int>())).Returns(new List<Diet>());
            _mockRepo.Setup(r => r.GetWorkoutsByUser(It.IsAny<int>())).Returns(new List<Workout>());

            // Act
            _controller.Index();

            // Assert
            _mockRepo.Verify(r => r.GetDietsByUser(1), Times.Once);
            _mockRepo.Verify(r => r.GetWorkoutsByUser(1), Times.Once);
        }

        #endregion

        #region GetMacroData Tests

        [Fact]
        public void GetMacroData_ShouldReturnCorrectTotals()
        {
            // Arrange
            var diets = new List<Diet>
            {
                new Diet { Proteins = 30, Carbohydrates = 40, Fats = 10, Calories = 370, UserId = 1, Id = 1, FoodName = "Meal 1" },
                new Diet { Proteins = 20, Carbohydrates = 30, Fats = 5, Calories = 245, UserId = 1, Id = 2, FoodName = "Meal 2" }
            };

            _mockRepo.Setup(r => r.GetDietsByUser(1)).Returns(diets);

            // Act
            var result = _controller.GetMacroData() as JsonResult;

            // Assert
            result.Should().NotBeNull();
            dynamic data = result!.Value!;
            ((int)data.totalProteins).Should().Be(50);
            ((int)data.totalCarbs).Should().Be(70);
            ((int)data.totalFats).Should().Be(15);
            ((int)data.totalCalories).Should().Be(615);
        }

        [Fact]
        public void GetMacroData_WithNoDiets_ShouldReturnZeros()
        {
            // Arrange
            _mockRepo.Setup(r => r.GetDietsByUser(1)).Returns(new List<Diet>());

            // Act
            var result = _controller.GetMacroData() as JsonResult;

            // Assert
            result.Should().NotBeNull();
            dynamic data = result!.Value!;
            ((int)data.totalProteins).Should().Be(0);
            ((int)data.totalCarbs).Should().Be(0);
            ((int)data.totalFats).Should().Be(0);
            ((int)data.totalCalories).Should().Be(0);
        }

        #endregion

        #region InsertDiet Tests

        [Fact]
        public void InsertDietToDatabase_WithValidModel_ShouldRedirectToIndex()
        {
            // Arrange
            var dietDto = new DietDto
            {
                FoodName = "Oatmeal",
                Proteins = 5,
                Fats = 3,
                Carbohydrates = 27,
                Calories = 150
            };

            // Act
            var result = _controller.InsertDietToDatabase(dietDto) as RedirectToActionResult;

            // Assert
            result.Should().NotBeNull();
            result!.ActionName.Should().Be("Index");
            _mockRepo.Verify(r => r.InsertDiet(It.Is<Diet>(d =>
                d.FoodName == "Oatmeal" &&
                d.UserId == 1 &&
                d.Proteins == 5)), Times.Once);
        }

        [Fact]
        public void InsertDietToDatabase_WithInvalidModel_ShouldReturnViewWithErrors()
        {
            // Arrange
            var dietDto = new DietDto { FoodName = "" };
            _controller.ModelState.AddModelError("FoodName", "Required");

            // Act
            var result = _controller.InsertDietToDatabase(dietDto) as ViewResult;

            // Assert
            result.Should().NotBeNull();
            result!.ViewName.Should().Be("InsertDiet");
            result.Model.Should().Be(dietDto);
            _mockRepo.Verify(r => r.InsertDiet(It.IsAny<Diet>()), Times.Never);
        }

        #endregion

        #region InsertWorkout Tests

        [Fact]
        public void InsertWorkoutToDatabase_WithValidModel_ShouldRedirectToIndex()
        {
            // Arrange
            var workoutDto = new WorkoutDto
            {
                WorkoutName = "Deadlifts",
                Reps = 5,
                Sets = 5,
                PersonalRecord = 405,
                Description = "Heavy lift"
            };

            // Act
            var result = _controller.InsertWorkoutToDatabase(workoutDto) as RedirectToActionResult;

            // Assert
            result.Should().NotBeNull();
            result!.ActionName.Should().Be("Index");
            _mockRepo.Verify(r => r.InsertWorkout(It.Is<Workout>(w =>
                w.WorkoutName == "Deadlifts" &&
                w.UserId == 1 &&
                w.Reps == 5)), Times.Once);
        }

        #endregion

        #region ViewDiet Tests

        [Fact]
        public void ViewDiet_WithValidId_ShouldReturnDiet()
        {
            // Arrange
            var diet = new Diet { Id = 1, UserId = 1, FoodName = "Eggs", Proteins = 6, Fats = 5, Carbohydrates = 1, Calories = 78 };
            _mockRepo.Setup(r => r.GetDietByUser(1, 1)).Returns(diet);

            // Act
            var result = _controller.ViewDiet(1) as ViewResult;

            // Assert
            result.Should().NotBeNull();
            var model = result!.Model as Diet;
            model.Should().NotBeNull();
            model!.FoodName.Should().Be("Eggs");
        }

        [Fact]
        public void ViewDiet_WithInvalidId_ShouldReturnNotFound()
        {
            // Arrange
            _mockRepo.Setup(r => r.GetDietByUser(999, 1)).Returns((Diet?)null);

            // Act
            var result = _controller.ViewDiet(999);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        #endregion

        #region UpdateDiet Tests

        [Fact]
        public void UpdateDiet_WithValidId_ShouldReturnDtoView()
        {
            // Arrange
            var diet = new Diet { Id = 1, UserId = 1, FoodName = "Steak", Proteins = 26, Fats = 15, Carbohydrates = 0, Calories = 250 };
            _mockRepo.Setup(r => r.GetDietByUser(1, 1)).Returns(diet);

            // Act
            var result = _controller.UpdateDiet(1) as ViewResult;

            // Assert
            result.Should().NotBeNull();
            var model = result!.Model as DietDto;
            model.Should().NotBeNull();
            model!.FoodName.Should().Be("Steak");
            model.Id.Should().Be(1);
            model.Proteins.Should().Be(26);
        }

        [Fact]
        public void UpdateDietToDatabase_WithValidModel_ShouldUpdateAndRedirect()
        {
            // Arrange
            var diet = new Diet { Id = 1, UserId = 1, FoodName = "Old Food", Proteins = 10, Fats = 5, Carbohydrates = 20, Calories = 100 };
            var dietDto = new DietDto { Id = 1, FoodName = "New Food", Proteins = 20, Fats = 10, Carbohydrates = 30, Calories = 200 };

            _mockRepo.Setup(r => r.GetDietByUser(1, 1)).Returns(diet);

            // Act
            var result = _controller.UpdateDietToDatabase(1, dietDto) as RedirectToActionResult;

            // Assert
            result.Should().NotBeNull();
            result!.ActionName.Should().Be("Index");
            _mockRepo.Verify(r => r.UpdateDiet(It.Is<Diet>(d =>
                d.FoodName == "New Food" &&
                d.Proteins == 20)), Times.Once);
        }

        #endregion

        #region DeleteDiet Tests

        [Fact]
        public void DeleteDiet_ShouldCallRepositoryAndRedirect()
        {
            // Act
            var result = _controller.DeleteDiet(1) as RedirectToActionResult;

            // Assert
            result.Should().NotBeNull();
            result!.ActionName.Should().Be("Index");
            _mockRepo.Verify(r => r.DeleteDietByUser(1, 1), Times.Once);
        }

        #endregion

        #region Security Tests

        [Fact]
        public void UpdateDietToDatabase_WithNonExistentDiet_ShouldReturnNotFound()
        {
            // Arrange
            var dietDto = new DietDto { Id = 999, FoodName = "Food", Proteins = 10, Fats = 5, Carbohydrates = 20, Calories = 100 };
            _mockRepo.Setup(r => r.GetDietByUser(999, 1)).Returns((Diet?)null);

            // Act
            var result = _controller.UpdateDietToDatabase(999, dietDto);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
            _mockRepo.Verify(r => r.UpdateDiet(It.IsAny<Diet>()), Times.Never);
        }

        #endregion
    }

    // Mock session helper
    public class MockHttpSession : ISession
    {
        private readonly Dictionary<string, byte[]> _sessionStorage = new();

        public bool IsAvailable => true;
        public string Id => "test-session";
        public IEnumerable<string> Keys => _sessionStorage.Keys;

        public void Clear() => _sessionStorage.Clear();
        public Task CommitAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task LoadAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
        public void Remove(string key) => _sessionStorage.Remove(key);
        public void Set(string key, byte[] value) => _sessionStorage[key] = value;
        public bool TryGetValue(string key, out byte[]? value) => _sessionStorage.TryGetValue(key, out value);
    }
}


