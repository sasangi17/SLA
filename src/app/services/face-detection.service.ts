import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class FaceDetectionService {

  private modelsLoaded = false;

  /**
   * Load Tiny Face Detector model
   */
  async loadModels(): Promise<void> {

    // Don't load the model again if already loaded
    if (this.modelsLoaded) {
      return;
    }

    try {

      // Dynamic import is important for Angular SSR
      const faceapi = await import('face-api.js');

      await faceapi.nets.tinyFaceDetector.loadFromUri(
        '/assets/models'
      );

      this.modelsLoaded = true;

      console.log(
        'Face detection model loaded successfully.'
      );

    } catch (error) {

      console.error(
        'Failed to load face detection model:',
        error
      );

      throw error;
    }
  }


  /**
   * Check whether the uploaded image contains
   * exactly ONE human face.
   */
  async validateSingleFace(
    file: File
  ): Promise<{
    valid: boolean;
    faceCount: number;
    message: string;
  }> {

    try {

      // Load model
      await this.loadModels();

      // Load face-api.js
      const faceapi =
        await import('face-api.js');

      // Convert uploaded File to HTMLImageElement
      const image =
        await this.fileToImage(file);


      // Detect ALL faces
      const detections =
        await faceapi.detectAllFaces(
          image,
          new faceapi.TinyFaceDetectorOptions({
            inputSize: 416,
            scoreThreshold: 0.5
          })
        );


      const faceCount =
        detections.length;


      console.log(
        'Detected faces:',
        faceCount
      );


      // ==========================================
      // NO FACE
      // ==========================================

      if (faceCount === 0) {

        return {
          valid: false,
          faceCount: 0,
          message:
            'No face was detected. Please upload a photo containing one person.'
        };
      }


      // ==========================================
      // MORE THAN ONE FACE
      // ==========================================

      if (faceCount > 1) {

        return {
          valid: false,
          faceCount,
          message:
            'Multiple faces were detected. Please upload a photo containing only one person.'
        };
      }


      // ==========================================
      // EXACTLY ONE FACE
      // ==========================================

      return {
        valid: true,
        faceCount: 1,
        message:
          'One face detected.'
      };

    } catch (error) {

      console.error(
        'Face detection error:',
        error
      );

      return {
        valid: false,
        faceCount: 0,
        message:
          'Unable to analyze the image. Please try another image.'
      };
    }
  }


  /**
   * Convert uploaded File into an HTMLImageElement
   */
  private fileToImage(
    file: File
  ): Promise<HTMLImageElement> {

    return new Promise(
      (resolve, reject) => {

        const image =
          new Image();

        const objectUrl =
          URL.createObjectURL(file);


        // Image loaded successfully
        image.onload = () => {

          URL.revokeObjectURL(
            objectUrl
          );

          resolve(image);
        };


        // Image failed to load
        image.onerror = () => {

          URL.revokeObjectURL(
            objectUrl
          );

          reject(
            new Error(
              'Unable to load image.'
            )
          );
        };


        image.src =
          objectUrl;
      }
    );
  }
}