import fs from 'fs';
import path from 'path';

const currentFilePath = path.resolve(process.argv[1]);
const outputDir = path.join(path.dirname(currentFilePath), '..', 'swa');

// clean up the output directory
fs.readdirSync(outputDir).forEach((item) => {
    if (item !== 'api') {
        const itemPath = path.join(outputDir, item);
        fs.rmSync(itemPath, { recursive: true, force: true });
    }
});

// copy the build dir to the output dir
const buildDir = path.join(path.dirname(currentFilePath), '..', 'build');
fs.readdirSync(buildDir).forEach((item) => {
    const itemPath = path.join(buildDir, item);
    if (fs.lstatSync(itemPath).isDirectory()) {
        const destPath = path.join(outputDir, item);
        fs.rmSync(destPath, { recursive: true, force: true });
        fs.renameSync(itemPath, destPath);
    }
});
