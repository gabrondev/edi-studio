# EDI Studio

O EDI Studio é uma aplicação desktop desenvolvida em C# e WPF para interpretação, visualização e edição de arquivos EDI no padrão PROCEDA, utilizados no intercâmbio de informações do setor de transportes.

A aplicação utiliza layouts definidos em JSON para interpretar arquivos TXT posicionais, apresentar seus registros e campos de forma legível, permitir edições e exclusões de registros e salvar o resultado no arquivo original ou em um novo arquivo.

## Status do projeto

**Alpha**

O EDI Studio está em fase inicial de desenvolvimento. A aplicação está disponível para uso experimental e testes, mas sua interface, arquitetura e comportamentos ainda podem sofrer alterações significativas.

## Executando o projeto

Requisitos:

- Windows.
- .NET 10 SDK.
- Visual Studio com suporte ao desenvolvimento desktop .NET.

Abra `EdiStudio.slnx`, defina o projeto `EdiStudio` como projeto de inicialização e execute a aplicação.

## Testes

Os testes estão no projeto `EdiStudio.Tests` e podem ser executados pelo Test Explorer do Visual Studio.

## Documentação

- [Glossário do domínio](docs/glossario-dominio.md)
- [Arquitetura](docs/arquitetura.md)