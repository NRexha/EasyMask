# Easy Mask
**Easy Mask** is a free Unity package I created that allows you to paint on your models directly in the editor, eliminating the need to switch to external software just to create masks. You can watch the demo video or read the explanatios below to see how it works!

## Demo
https://github.com/user-attachments/assets/0ffd1d37-f8cd-41f2-99e7-a61eaabb222d

## Installation
1. Copy the git url of this repository
![image](https://github.com/user-attachments/assets/8f295f7a-f391-4915-9763-dbe33acc0e5f)

2. In Unity, install the package by pasting the git url
![install_tutorial](https://github.com/user-attachments/assets/918cd466-6a7f-40eb-a182-d30b0b93f102)

3. You can now open the tool by going under Tools/Easy Mask
![image](https://github.com/user-attachments/assets/3528191a-8f8e-416d-87df-0167b3c5e9ed)

## Overview
The tool runs smoothly thanks to the use of a compute shader. Most of the heavy logic is run on the gpu, guaranteeing more than 200fps while painting even in low end setups.
![overview](https://github.com/user-attachments/assets/d48887f4-1bc1-46a2-9095-0082305d6046)


- **Paint on Any Model**  
  Paint directly onto 3D models in the Unity Editor, the only requirement is them having a mesh renderer attached of course.

- **Multi-Channel Masking**  
  Paint on specific texture channels, enabling up to 3 masks in a single texture.

- **Symmetry Painting**  
  The tool provides a built-in symmetry option.

- **Customizable Brushes**  
  Tailor your brush to your needs with the following settings: shape (you can even use your own alphas), size, smoothness, opacity and repetition

- **Output**  
  Choose your desired output texture resolution(up to 4k) and format (PNG and TGA) to suit your project requirements.
 
## Basic Controls
- You can use your brush by holding the left mouse button
- You can switch between brush/eraser
- You can add a symmetry point on your model with the right mouse button

## License
You can download this tool and make whatever you want with it. The goal is to keep it available for everyone, so please don't sell it (I would be surprised that someone would actually buy it, but still).

Happy painting! 

