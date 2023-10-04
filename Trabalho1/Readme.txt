// Trabalho 1 – Shader
// CGA - 2023/2
// Gustavo Machado de Freitas
//
// Programa em C++, utilizando a API OpenGL 4.x, para simular um terreno 
// formado por N² patches, dispostos em um grid NxN. Desenvolvido usando
// como base a demo glsl40_tessellation_displacement_mapping.
// 
// Use WASD para mover a camera pelo mapa e mouse para rotacionar a camera.
// Use R/F para aumentar/diminuir o tessellation level máximo.
// Use E para ativar/desativar wireframe.
// Use T para ativar/desativar tessellation no centro do plano.
// Use C para ativar/desativar tessellation por distancia da camera.

O que foi implementado:

● Terreno formado por 9 patches (ou mais).
● Cada pode ser refinado com tesselation shader usando um diferente nível de
refinamento.
● Os vértices nas bordas de patches vizinhos podem ser iguais.
● Uso de geometry shader para gerar as normais de cada triângulo gerado.
● Uso de textura e função procedural para fazer a perturbação de altura do terreno em cada vértice.
● Terreno iluminado por duas fontes luminosas em movimento.
● O nível de refinamento pode mudar em função da distância da câmera.
● Pode visualizar o terreno em wireframe e com preenchimento.
● Pode também ajustar o nível de refinamento do patch central, para forçar a ocorrência de T-Vertex.
● Animação do terreno, como se fosse um mar.
