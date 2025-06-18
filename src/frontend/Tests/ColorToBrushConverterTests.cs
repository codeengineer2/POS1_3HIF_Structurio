using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Structurio.Classes;

namespace Tests
{
    [TestClass]
    public class ColorToBrushConverterTests
    {
        private ColorToBrushConverter converter = null!;

        [TestInitialize]
        public void Setup()
        {
            converter = new ColorToBrushConverter();
        }

        [TestMethod]
        public void Convert_Valid_Color()
        {
            // Arrange
            string input = "#FF0000";

            // Act
            var result = converter.Convert(input, null!, null!, null!) as SolidColorBrush;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(Colors.Red, result!.Color);
        }

        [TestMethod]
        public void Convert_Invalid_Color()
        {
            // Arrange
            var input = 123;

            // Act
            var result = converter.Convert(input, null!, null!, null!);

            // Assert
            Assert.AreEqual(Brushes.DarkGray, result);
        }

        [TestMethod]
        public void Convert_Invalid()
        {
            // Arrange
            object? input = null;

            // Act
            var result = converter.Convert(input!, null!, null!, null!);

            // Assert
            Assert.AreEqual(Brushes.DarkGray, result);
        }

        [TestMethod]
        [ExpectedException(typeof(System.NotImplementedException))]
        public void Convert_Back()
        {
            // Act & Assert
            converter.ConvertBack(null!, null!, null!, null!);
        }
    }
}