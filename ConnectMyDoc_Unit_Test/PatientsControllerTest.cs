using ConnectMyDoc_API_Layer.Controllers;
using ConnectMyDoc_API_Layer.Models;
using ConnectMyDoc_Domain_Layer.DTOs;
using ConnectMyDoc_Domain_Layer.Manager;
using ConnectMyDoc_Domain_Layer.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace ConnectMyDoc_Unit_Test
{
    [TestClass]
    public class PatientsControllerTest
    {
        private Mock<IPatientManager> _mockPatientManager;
        private Mock<IMessageService> _mockMessageService;
        private PatientsController _controller;

        [TestInitialize]
        public void Setup()
        {
            _mockPatientManager = new Mock<IPatientManager>();
            _mockMessageService = new Mock<IMessageService>();
            _controller = new PatientsController( _mockPatientManager.Object, _mockMessageService.Object);
        }


        [TestMethod]
        public async Task GetPatientById_ReturnsOkResult_WhenPatientExists()
        {
            // Arrange
            int patientId = 1;
            var patientDto = new PatientDTO { PatientId = patientId, PatientName = "John Doe" };
            _mockPatientManager.Setup(service => service.GetPatientByIdAsync(patientId))
                .ReturnsAsync(patientDto);

            // Act
            var result = await _controller.GetPatientById(patientId);

            // Assert
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);

            var returnedPatient = okResult.Value as PatientDTO;
            Assert.IsNotNull(returnedPatient);
            Assert.AreEqual(patientId, returnedPatient.PatientId);
            Assert.AreEqual("John Doe", returnedPatient.PatientName);
        }
/*
        [TestMethod]
        public async Task GetPatientById_ReturnsNotFound_WhenPatientDoesNotExist()
        {
            // Arrange
            int patientId = 2;
            _mockPatientService.Setup(service => service.GetPatientByIdAsync(patientId))
                .ThrowsAsync(new KeyNotFoundException());

            // Act
            var result = await _controller.GetPatientById(patientId);

            // Assert
            var notFoundResult = result.Result as NotFoundResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode);
        }
*/


        [TestMethod]
        public async Task GetAllPatients_ReturnsOkResult_WithPaginatedPatients()
        {
            // Arrange
            int pageNumber = 1;
            int pageSize = 20;
            var patients = new List<PatientDTO>
            {
                new PatientDTO { PatientId = 1, PatientName = "John Doe" },
                new PatientDTO { PatientId = 2, PatientName = "Jane Smith" }
            };
            int totalCount = 2;

            _mockPatientManager.Setup(service => service.GetAllPatientsAsync(pageNumber, pageSize))
                .ReturnsAsync((patients, totalCount));

            // Act
            var result = await _controller.GetAllPatients(pageNumber, pageSize);

            // Assert
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);

            var paginatedResult = okResult.Value as PaginatedResult<PatientDTO>;
            Assert.IsNotNull(paginatedResult);

            Assert.AreEqual(2, paginatedResult.Items.Count());
            Assert.AreEqual("John Doe", paginatedResult.Items.First().PatientName);
            Assert.AreEqual("Jane Smith", paginatedResult.Items.Last().PatientName);

            Assert.AreEqual(totalCount, paginatedResult.TotalCountOfPatients);
            Assert.AreEqual(pageNumber, paginatedResult.PageNumber);
            Assert.AreEqual(pageSize, paginatedResult.PageSize);
        }

        [TestMethod]
        public async Task GetAllPatients_ReturnsEmptyList_WhenNoPatients()
        {
            // Arrange
            int pageNumber = 1;
            int pageSize = 20;
            var patients = new List<PatientDTO>();
            int totalCount = 0;

            _mockPatientManager.Setup(service => service.GetAllPatientsAsync(pageNumber, pageSize))
                .ReturnsAsync((patients, totalCount));

            // Act
            var result = await _controller.GetAllPatients(pageNumber, pageSize);

            // Assert
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);

            var paginatedResult = okResult.Value as PaginatedResult<PatientDTO>;
            Assert.IsNotNull(paginatedResult);

            Assert.AreEqual(0, paginatedResult.Items.Count());
            Assert.AreEqual(totalCount, paginatedResult.TotalCountOfPatients);
            Assert.AreEqual(pageNumber, paginatedResult.PageNumber);
            Assert.AreEqual(pageSize, paginatedResult.PageSize);
        }


        /*[TestMethod]
        public async Task DeletePatient_SuccessfulDeletion_ReturnsOk()
        {
            // Arrange
            int patientId = 1;
            _mockPatientManager.Setup(service => service.DeletePatientByIdAsync(patientId)).ReturnsAsync(true);

            // Act
            var result = await _controller.DeletePatientById(patientId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            Assert.AreEqual("Succesfully deleted", okResult.Value);
        }
*/
        /*[TestMethod]
        public async Task DeletePatient_PatientNotFound_ReturnsNotFound()
        {
            // Arrange
            int patientId = 1;
            _mockPatientManager.Setup(service => service.DeletePatientByIdAsync(patientId)).ReturnsAsync(false);

            // Act
            var result = await _controller.DeletePatientById(patientId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
            var notFoundResult = result as NotFoundObjectResult;
            Assert.AreEqual("Patient with id :1 not found", notFoundResult.Value);
        }
*/
        /*[TestMethod]
        public async Task DeletePatient_InternalServerError_ReturnsServerError()
        {
            // Arrange
            int patientId = 1;
            _mockPatientManager.Setup(service => service.DeletePatientByIdAsync(patientId)).Throws(new Exception());

            // Act
            var result = await _controller.DeletePatientById(patientId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ObjectResult));
            var objectResult = result as ObjectResult;
            Assert.AreEqual(500, objectResult.StatusCode);
            Assert.AreEqual("An internal server error occurred.", objectResult.Value);
        }*/

        /*[TestMethod]
        public async Task DeletePatient_InvalidPatientId_ReturnsBadRequest()
        {
            // Arrange
            int patientId = -1;

            // Act
            var result = await _controller.DeletePatientById(patientId);

            // Assert
            // Assuming validation for invalid ID is added in the method.
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }*/


        [TestMethod]
        public async Task AddPatient_ReturnsOkResult_WhenValidPatientDTO()
        {
            // Arrange
            var patientDTO = new PatientDTO
            {
                PatientName = "John Doe",
                Age = 25,
                Dob = new DateTime(1998, 1, 1),
                Email = "john.doe@example.com",
                Phone = "1234567890",
                StreetAddress = "123 Main St",
                City = "New York",
                State = "NY",
                Country = "USA",
                ZipCode = "10001"
            };

            _mockPatientManager.Setup(service => service.AddPatientAsync(patientDTO))
                .ReturnsAsync(patientDTO);

            // Act
            var result = await _controller.AddPatient(patientDTO);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(StatusCodes.Status200OK, okResult.StatusCode);
            Assert.AreEqual(patientDTO, okResult.Value);
            _mockPatientManager.Verify(service => service.AddPatientAsync(patientDTO), Times.Once);
        }

        [TestMethod]
        public async Task AddPatient_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            var patientDTO = new PatientDTO();
            _controller.ModelState.AddModelError("PatientName", "PatientName is required.");

            // Act
            var result = await _controller.AddPatient(patientDTO);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);
            Assert.IsInstanceOfType(badRequestResult.Value, typeof(SerializableError));
            _mockPatientManager.Verify(service => service.AddPatientAsync(It.IsAny<PatientDTO>()), Times.Never);
        }
/*
        [TestMethod]
        public async Task AddPatient_ReturnsInternalServerError_WhenServiceThrowsException()
        {
            // Arrange
            var patientDTO = new PatientDTO
            {
                PatientName = "John Doe",
                Age = 25,
                Dob = new DateTime(1998, 1, 1),
                Email = "john.doe@example.com",
                Phone = "1234567890",
                StreetAddress = "123 Main St",
                City = "New York",
                State = "NY",
                Country = "USA",
                ZipCode = "10001"
            };

            _mockPatientService.Setup(service => service.AddPatientAsync(It.IsAny<PatientDTO>()))
                .ThrowsAsync(new Exception("An error occurred while adding the patient"));

            // Act
            var result = await _controller.AddPatient(patientDTO);

            // Assert
            var internalServerErrorResult = result as ObjectResult;
            Assert.IsNotNull(internalServerErrorResult);
            Assert.AreEqual(StatusCodes.Status500InternalServerError, internalServerErrorResult.StatusCode);
            Assert.AreEqual("An error occurred while adding the patient", internalServerErrorResult.Value);
            _mockPatientService.Verify(service => service.AddPatientAsync(patientDTO), Times.Once);
        }
*/
    
        [TestMethod]
        public async Task UpdatePatient_ReturnsOkResult_WhenUpdateIsSuccessful()
        {
            // Arrange
            int patientId = 1;
            var patientDto = new PatientDTO
            {
                PatientId = patientId,
                PatientName = "John Doe",
                Email = "john.doe@example.com",
                Phone = "123-456-7890",
                Age = 30,
                Dob = DateTime.Now.AddYears(-30),
                Gender = "Male",
                PreferredStartTime = DateTime.Now.AddHours(1),
                PreferredEndTime = DateTime.Now.AddHours(2),
                CreatedBy = 1,
                LastModifiedBy = 1,
                StreetAddress = "123 Main St"
            };

            _mockPatientManager
                .Setup(service => service.UpdatePatient(patientDto, patientId))
                .ReturnsAsync(patientDto);

            // Act
            var result = await _controller.UpdatePatient(patientId, patientDto);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult, "Expected OkObjectResult");
            Assert.AreEqual(200, okResult.StatusCode, "Expected status code 200");

            var returnedPatient = okResult.Value as PatientDTO;
            Assert.IsNotNull(returnedPatient, "Expected PatientDTO");
            Assert.AreEqual(patientId, returnedPatient.PatientId, "Patient ID mismatch");
            Assert.AreEqual("John Doe", returnedPatient.PatientName, "Patient name mismatch");
        }

        [TestMethod]
        public async Task UpdatePatient_ReturnsBadRequest_WhenPatientDtoIsInvalid()
        {
            // Arrange
            int patientId = 1;
            PatientDTO patientDto = null; // Invalid DTO

            // Act
            var result = await _controller.UpdatePatient(patientId, patientDto);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult, "Expected BadRequestObjectResult");
            Assert.AreEqual(400, badRequestResult.StatusCode, "Expected status code 400");
            Assert.AreEqual("Patient data is null.", badRequestResult.Value, "Expected error message mismatch");
        }

       /* [TestMethod]
        public async Task UpdatePatient_ReturnsNotFound_WhenPatientDoesNotExist()
        {
            // Arrange
            int patientId = 1;
            var patientDto = new PatientDTO
            {
                PatientId = patientId,
                PatientName = "John Doe",
                Email = "john.doe@example.com",
                Phone = "123-456-7890",
                Age = 30,
                Dob = DateTime.Now.AddYears(-30),
                Gender = "Male",
                PreferredStartTime = DateTime.Now.AddHours(1),
                PreferredEndTime = DateTime.Now.AddHours(2),
                CreatedBy = 1,
                LastModifiedBy = 1,
                StreetAddress = "123 Main St"
            };

            _mockPatientService
                .Setup(service => service.UpdatePatient(patientDto, patientId))
                .ReturnsAsync((PatientDTO)null);

            // Act
            var result = await _controller.UpdatePatient(patientId, patientDto);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult, "Expected NotFoundObjectResult");
            Assert.AreEqual(404, notFoundResult.StatusCode, "Expected status code 404");
            Assert.AreEqual("Patient not found.", notFoundResult.Value, "Expected error message mismatch");
        }*/
/*
        [TestMethod]
        public async Task UpdatePatient_ReturnsInternalServerError_WhenExceptionIsThrown()
        {
            // Arrange
            int patientId = 1;
            var patientDto = new PatientDTO
            {
                PatientId = patientId,
                PatientName = "John Doe",
                Email = "john.doe@example.com",
                Phone = "123-456-7890",
                Age = 30,
                Dob = DateTime.Now.AddYears(-30),
                Gender = "Male",
                PreferredStartTime = DateTime.Now.AddHours(1),
                PreferredEndTime = DateTime.Now.AddHours(2),
                CreatedBy = 1,
                LastModifiedBy = 1,
                StreetAddress = "123 Main St"
            };

            _mockPatientService
                .Setup(service => service.UpdatePatient(patientDto, patientId))
                .ThrowsAsync(new Exception("Some error occurred"));

            // Act
            var result = await _controller.UpdatePatient(patientId, patientDto);

            // Assert
            var internalServerErrorResult = result as ObjectResult;
            Assert.IsNotNull(internalServerErrorResult, "Expected ObjectResult");
            Assert.AreEqual(500, internalServerErrorResult.StatusCode, "Expected status code 500");
            Assert.AreEqual("Internal server error: Some error occurred", internalServerErrorResult.Value, "Expected error message mismatch");
        }
    */
    }
}