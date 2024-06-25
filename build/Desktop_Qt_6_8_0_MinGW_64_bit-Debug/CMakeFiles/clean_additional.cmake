# Additional clean files
cmake_minimum_required(VERSION 3.16)

if("${CONFIG}" STREQUAL "" OR "${CONFIG}" STREQUAL "Debug")
  file(REMOVE_RECURSE
  "CMakeFiles\\axuanTools_autogen.dir\\AutogenUsed.txt"
  "CMakeFiles\\axuanTools_autogen.dir\\ParseCache.txt"
  "axuanTools_autogen"
  )
endif()
