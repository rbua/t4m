function translate_input() {
	const inputText = document.getElementById("inputText").value;
	const url = "https://localhost:7177/Translate/ru/en/" + encodeURIComponent(inputText);
	const options = {
	  method: "POST",
	  headers: { "Content-Type": "application/json" }
	};
  
	fetch(url, options)
	  .then(response => response.json())
	  .then(data => {
		const audioData = data.pronunciation.audio;
		const audioCtx = new AudioContext();
		audioCtx.decodeAudioData(new Uint8Array(audioData).buffer).then(audioBuffer => {
		  const audioSource = audioCtx.createBufferSource();
		  audioSource.buffer = audioBuffer;
		  audioSource.connect(audioCtx.destination);
		  audioSource.start();
		});
		document.getElementById("result").innerHTML = data.translation.translation;
	  });
  }

document.getElementById("translateButton").addEventListener("click", translate_input);

document.addEventListener("dblclick", function(event) {  });
  