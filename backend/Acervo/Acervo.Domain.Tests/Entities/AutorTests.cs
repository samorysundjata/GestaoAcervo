using System;
using Xunit;
using Acervo.Domain.Entities;

namespace Acervo.Domain.Tests.Entities
{
    public class AutorTests
    {
        [Fact]
        public void Constructor_ShouldInitializePropertiesAndCollections()
        {
            // Arrange
            var nome = "Fernando Pessoa";
            var email = "fernando@example.com";

            // Act
            var autor = new Autor(nome, email);

            // Assert
            Assert.NotEqual(Guid.Empty, autor.Id);
            Assert.Equal(nome, autor.Nome);
            Assert.Equal(email, autor.Email);
            Assert.NotNull(autor.Livros);
            Assert.Empty(autor.Livros);
        }

        [Fact]
        public void Update_ShouldChangeNomeAndEmail_KeepIdAndCollections()
        {
            // Arrange
            var originalNome = "Fernando Pessoa";
            var originalEmail = "fernando@example.com";
            var autor = new Autor(originalNome, originalEmail);
            var originalId = autor.Id;

            var novoNome = "Pessoa Renovado";
            var novoEmail = "pessoa.renovado@example.com";

            // Act
            autor.Update(novoNome, novoEmail);

            // Assert
            Assert.Equal(originalId, autor.Id);
            Assert.Equal(novoNome, autor.Nome);
            Assert.Equal(novoEmail, autor.Email);
            Assert.NotNull(autor.Livros);
            Assert.Empty(autor.Livros);
        }
    }
}