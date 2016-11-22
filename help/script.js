function makeScrollSmooth() {
  var links = document.getElementsByClassName('smoothScroll');
  for (var i = 0; i < links.length; ++i) {
    links[i].onclick = scrollIntoView.bind(links[i]);
  }
}

function scrollIntoView() {
  document.querySelector(this.hash).scrollIntoView({ 
    behavior: 'smooth' 
  });
  location.hash = this.hash;
  return false;
}