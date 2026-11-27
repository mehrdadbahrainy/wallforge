# WallForge branding

Source: owner-provided `SharedSpace/WallForge/wallforge-brand-v0.1.0.zip`.
Approved ZIP SHA256: `2596150f68ef2ec6110462f49e03f49120d52c5e6497b01993f72468733abb10`.
Original PNG/ICO files remain unchanged, covered by approved-assets.sha256.json.

## Transparent SVG update — 2026-11-27
The owner subsequently requested transparent SVG and a redesigned header. The supplied raster geometry was traced into actual SVG paths with VTracer1.0.0-alpha.4, after classifying opaque source pixels into navy/orange. Texture is simplified to flat #041C32 and #FF5800. This is a vector derivative, not an original artist-supplied vector or a byte-identical conversion. There are no embedded bitmaps, background rectangles, scripts, external references or font dependencies. Shield, flame, brick layout and lettering remain based on the approved artwork.

- wallforge-horizontal.svg: navy/orange transparent wordmark.
- wallforge-symbol.svg: navy/orange transparent shield.
- wallforge-horizontal-light.svg: same traced geometry, navy changed to #EDF3FA for contrast on dark surfaces. Used in header and sign-in.

Removed opaque CSS logo backing and the slogan beneath the header logo. The header now separates brand, Host console context, read-only mode and user actions; mobile hides secondary context while preserving username, role and sign-out. No authentication or backend behavior changed.
vector-assets.sha256.json records derivative hashes. tests/branding/verify-assets.py checks original bytes, source/build SVG hashes, path-only structure and logo wiring.

The earlier RC1 tarball remains immutable and contains the previous PNG-based header. It does not include this subsequent design update; a new package is required before publishing these changes. Stable release gates are unchanged.
