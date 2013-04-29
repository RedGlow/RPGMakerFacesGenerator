#!/usr/bin/env python

#   Delayerize
#   Copyright (C) 2013  Mattia Belletti <mattia.belletti@gmail.com>
#
#   This filters performs multiple tasks in order to create the data
#   for RPG Face Maker.
#   - It splits an image into layer
#   - For each layer, saves its position and size into an offsets.txt file
#   - Each layer is HSVA decomposed, and then RGBA re-composed (so that now
#     the R channel contains instead the H component, the G channel the S
#     component, the B channel the V component, and the A channel remains
#     the same)
#   The images are thus no longer visualizable as they are, but the HSV shift
#   filter of RPG Face Maker can skip the HSV decomposition (essential to produce
#   a shader short enough to fit into the PSv2 limits).
#
#   This program is free software: you can redistribute it and/or modify
#   it under the terms of the GNU General Public License as published by
#   the Free Software Foundation; either version 3 of the License, or
#   (at your option) any later version.
#
#   This program is distributed in the hope that it will be useful,
#   but WITHOUT ANY WARRANTY; without even the implied warranty of
#   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
#   GNU General Public License for more details.
#
#   You should have received a copy of the GNU General Public License
#   along with this program.  If not, see <http://www.gnu.org/licenses/>.

from gimpfu import *
import os.path

gettext.install("gimp20-python", gimp.locale_directory, unicode=True)

def process(dirname, offsets_file, img, layer, stack):
	# decompose in HSV and recompose in RGB
	decomposed_image = pdb['plug-in-decompose'](img, layer, 'HSV', 1)[0]
	alpha_image = pdb['plug-in-decompose'](img, layer, 'Alpha', 1)[0]
	new_image = pdb['plug-in-drawable-compose'](None,
		decomposed_image.layers[0],
		decomposed_image.layers[1],
		decomposed_image.layers[2],
		alpha_image.layers[0],
		'RGBA')
	pdb.gimp_image_delete(decomposed_image)
	pdb.gimp_image_delete(alpha_image)
	fbase = '_'.join(map(str, stack))
	fname = os.path.join(dirname, fbase) + '.png'
	pdb.gimp_file_save(new_image, new_image.layers[0], fname, fname)
	pdb.gimp_image_delete(new_image)
	# save the offset
	offsets = layer.offsets
	width = layer.width
	height = layer.height
	offsets_file.write('%s %d %d %d %d\n' % (fbase, int(offsets[0]), int(offsets[1]), int(width), int(height)))
	# process children
	for i, child in enumerate(layer.children):
		process(dirname, offsets_file, img, child, stack + [i])

def delayerize(img, layer, dirname):
	with open(os.path.join(dirname, 'offsets.txt'), 'wt') as offsets_file:
		for i, layer in enumerate(img.layers):
			process(dirname, offsets_file, img, layer, [i])

register(
    "python-fu-delayerize",
	"Save the various layers of the image in different files",
	"Splits the image in its layers and sublayers",
	"Mattia Belletti",
	"Mattia Belletti",
	"2013",
	"Delayerize",
	"*",
    [
        (PF_IMAGE, "image",       "Input image", None),
        (PF_DRAWABLE, "drawable", "Input drawable", None),
		(PF_DIRNAME, 'dest_dir', 'Where to save all the files', '')
    ],
    [],
    delayerize,
    menu="<Image>/Filters",
    domain=("gimp20-python", gimp.locale_directory)
    )

main()