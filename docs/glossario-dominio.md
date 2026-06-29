# Glossário do domínio

## Layout EDI

Descrição estrutural de um documento PROCEDA. Define os tipos de
registro permitidos, seus tamanhos, campos, posições e formatos.

Código atual: `Layout`.

## Definição de campo

Metadados que descrevem onde um campo se encontra dentro de um
registro, seu tamanho, formato e obrigatoriedade.

Código atual: `Campo`.

Não representa o valor encontrado em um documento concreto.

## Documento EDI

Representação em memória de um arquivo EDI carregado pelo usuário.
Contém linhas, registros reconhecidos, valores e futuros diagnósticos.

Código atual: `ArquivoEdiParseado`.

## Registro EDI

Ocorrência concreta de um tipo de registro dentro de um documento.

Código atual: `RegistroEdiNode`.

## Definição de tipo de registro

Metadados do layout que descrevem uma categoria de linha EDI.

A definição contém o identificador de três caracteres, o tamanho
esperado, os campos e sua possível relação hierárquica com outro tipo
de registro.

Código atual: `TipoRegistro`.

Não representa uma ocorrência concreta encontrada no documento.

Nome proposto: `DefinicaoRegistroEdi`.

## Valor de campo EDI

Valor original e valor atual de um campo dentro de um registro concreto.

Código atual: `CampoEdiValor`.

Nome proposto: `ValorCampoEdi`.

## Linha física

Linha existente no arquivo TXT original, reconhecida ou não pelo layout.

Observação: o modelo atual não preserva todas as linhas físicas.

## Diagnóstico

Erro, aviso ou informação produzido durante a análise do documento.
Deve indicar severidade, linha, posição, código e mensagem.

Ainda não implementado.

## Reparo

Operação individual ou em massa que modifica o documento para corrigir
um ou mais diagnósticos.

Ainda não implementado.