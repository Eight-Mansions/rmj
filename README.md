# R?MJ The Mystery Hospital
## Building
- These instructions expect a Windows environment at this time.

### First time prep
1. In the `cd/` folder, drag your `.bin` file for disc 1 onto `prepare-clean-bin_rmj1.bat` to extract the original game files for building.
2. Repeat for disc 2 on `prepare-clean-bin_rmj2.bat`.

### Build
- To build, run either `build_disc_1.bat` or `build_disc_2.bat` in the root folder. Then your bin/cue will be in the cd folder.

To create a patch release, run `create_patch.bat` and an xdelta patch will be created under `release\patch_data`.
