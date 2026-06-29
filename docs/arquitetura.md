# Arquitetura do EDI Studio

## Objetivo do produto

Diagnosticar, visualizar e reparar documentos EDI PROCEDA, incluindo
arquivos fora do padrão.

## Fluxo atual

Layout JSON → Repository → Layout
TXT → Parser → Documento parseado
Documento → Renderer → Interface
Documento → Writer → TXT

## Componentes atuais

### MainWindowViewModel
Coordena abertura, seleção, edição, exclusão, salvamento e status.

### EdiParserService
Interpreta linhas conforme o layout.

### EdiWriterService
Reconstrói e grava o arquivo.

## Limitações conhecidas

- O MainWindowViewModel possui responsabilidades demais.
- Dependências concretas são criadas dentro do ViewModel.
- Linhas inválidas não são preservadas.
- Models misturam domínio e apresentação.
- A política de erros e notificações não está definida.

## Direção arquitetural

- Aplicar injeção de dependência.
- Dividir a interface em UserControls.
- Criar ViewModels menores e coesos.
- Separar documento, layout e apresentação.
- Preservar todas as linhas físicas.
- Implementar diagnósticos e reparos em massa.

## Próximos passos

- Adicionar testes do parser e writer.
- Definir nomes dos modelos.
- Introduzir injeção pelo construtor.
- Extrair responsabilidades do MainWindowViewModel.