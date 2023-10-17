// Trabalho 2 – Spatial Hashing
// CGA - 2023/2
// Gustavo Machado de Freitas

// Programa desenvolvido na Unity Engine

// Use os botões e campos na interface para configurar os testes.
// O botão Exportar Valores salva as configurações e resultados no arquivo em RaizDoProjeto/Assets/Data/data.csv

// Ferramentas de debug:
Apertar D com o mouse sobre um círculo permite mover ele e testar colisões.
Ativar os gizmos da Unity permitem observar o grid.


O que foi implementado:

● Curva criada de forma randômica com n pontos de controle.
● Distribuição aleatória de m círculos de raio r.
● Duas técnicas para determinar se os círculos estão sobre a curva:
	1. com força bruta (todos com todos)
	2. com o algoritmo de hash espacial.
● Comparação dos tempos de execução, considerando diferentes tamanhos de células de hash e
diferentes conjuntos de dados.
● Visualização dos círculos e da curva. 
● O programa bem interativo.

Extras:
● Função de zoom sobre o plano com uso do mouse.
● Linhas e círculos fora da câmera não pesam a aplicação.
● Ferramenta de exportação das configurações e resultados do teste em um .csv
