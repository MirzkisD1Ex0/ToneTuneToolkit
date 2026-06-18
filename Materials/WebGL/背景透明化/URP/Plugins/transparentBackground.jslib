// Create new module that we will inject into framework.js file
var transparentBackground = {
// Create a method glClear - this will overwrite default behaviour
glClear: function(mask)
{
// If mask of the layer is set to "depth only"
if (mask == 0x00004000)
{
var v = GLctx.getParameter(GLctx.COLOR_WRITEMASK);
// And the color is "transparent"
if (!v[0] && !v[1] && !v[2] && v[3])
// We do nothing
return;
}
// else we clear the mask to prevent tearing
GLctx.clear(mask);
}
};
// Merge our module into framework.js
mergeInto(LibraryManager.library, transparentBackground);