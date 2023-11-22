from xml.dom import minidom
from svgpathtools import svg2paths, wsvg, Path, Line

doc = minidom.parse('inter.svg') 

paths, attributes = svg2paths('inter.svg')

points = []
for path in paths:
    for segment in path:
        if isinstance(segment, Line):
            points.append((segment.start.real, segment.start.imag))
            points.append((segment.end.real, segment.end.imag))
            
with open('src/Points.h', 'w+') as f:
    f.write('#ifndef POINTS_H_INCLUDED\n'
            '#define POINTS_H_INCLUDED\n'
            '\n'
            '#include <vector>\n'
            '\n'
            'const std::vector<double> points = {\n')

    for point in points:
        f.write('   {}, {},\n'.format(point[0], point[1]))

    f.write('};\n'
            '\n'
            '#endif //POINTS_H_INCLUDED')

doc.unlink()