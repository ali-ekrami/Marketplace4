/* =========================================================
   TAGR MARKETPLACE - 3D DYNAMIC TILT & THEME SWITCHER ENGINE
   ========================================================= */

(function () {
    'use strict';

    // 1. Initialize Theme Switcher immediately & on DOM load
    function initTheme() {
        const themeToggleBtn = document.getElementById('themeToggleBtn');
        const currentTheme = document.documentElement.getAttribute('data-theme') ||
            localStorage.getItem('tagr-theme') ||
            (window.matchMedia('(prefers-color-scheme: dark)').matches ? 'dark' : 'light');

        document.documentElement.setAttribute('data-theme', currentTheme);
        updateThemeUI(currentTheme);

        if (themeToggleBtn) {
            const newBtn = themeToggleBtn.cloneNode(true);
            themeToggleBtn.parentNode.replaceChild(newBtn, themeToggleBtn);

            newBtn.addEventListener('click', (e) => {
                e.preventDefault();
                const active = document.documentElement.getAttribute('data-theme');
                const next = active === 'dark' ? 'light' : 'dark';

                document.documentElement.setAttribute('data-theme', next);
                localStorage.setItem('tagr-theme', next);
                updateThemeUI(next);

                // Add 3D click spin animation
                const icon = newBtn.querySelector('.tagr-theme-icon-container');
                if (icon) {
                    icon.classList.add('tagr-spin-pop');
                    setTimeout(() => icon.classList.remove('tagr-spin-pop'), 450);
                }
            });
        }
    }

    function updateThemeUI(theme) {
        const sunIcons = document.querySelectorAll('.tagr-sun-icon');
        const moonIcons = document.querySelectorAll('.tagr-moon-icon');

        if (theme === 'dark') {
            sunIcons.forEach(el => el.style.display = 'inline-flex');
            moonIcons.forEach(el => el.style.display = 'none');
        } else {
            sunIcons.forEach(el => el.style.display = 'none');
            moonIcons.forEach(el => el.style.display = 'inline-flex');
        }
    }

    // 2. High-Performance 3D Dynamic Tilt & Specular Lighting
    function init3DTilt() {
        const targetSelector = '.product-card, .category-card, .floating-card, .tagr-user-chip, .auth-card, [data-tilt], .card:not(.no-tilt), .tagr-3d-card';
        const elements = document.querySelectorAll(targetSelector);

        elements.forEach((card) => {
            if (!card.querySelector('.tagr-3d-glare')) {
                const glare = document.createElement('div');
                glare.className = 'tagr-3d-glare';
                card.style.position = card.style.position || 'relative';
                card.appendChild(glare);
            }

            const glare = card.querySelector('.tagr-3d-glare');

            card.addEventListener('mousemove', (e) => {
                const rect = card.getBoundingClientRect();
                const x = e.clientX - rect.left;
                const y = e.clientY - rect.top;

                const centerX = rect.width / 2;
                const centerY = rect.height / 2;

                const rotateX = ((y - centerY) / centerY) * -12;
                const rotateY = ((x - centerX) / centerX) * 12;

                card.style.transform = `perspective(1100px) rotateX(${rotateX.toFixed(2)}deg) rotateY(${rotateY.toFixed(2)}deg) translateZ(8px) scale3d(1.02, 1.02, 1.02)`;

                if (glare) {
                    const percentX = (x / rect.width) * 100;
                    const percentY = (y / rect.height) * 100;
                    glare.style.opacity = '1';
                    glare.style.background = `radial-gradient(circle at ${percentX}% ${percentY}%, rgba(255, 255, 255, 0.22) 0%, rgba(255, 255, 255, 0) 65%)`;
                }
            });

            card.addEventListener('mouseleave', () => {
                card.style.transition = 'transform 0.45s cubic-bezier(0.16, 1, 0.3, 1)';
                card.style.transform = 'perspective(1100px) rotateX(0deg) rotateY(0deg) translateZ(0px) scale3d(1, 1, 1)';
                if (glare) {
                    glare.style.opacity = '0';
                    glare.style.transition = 'opacity 0.4s ease';
                }
            });

            card.addEventListener('mouseenter', () => {
                card.style.transition = 'transform 0.1s ease-out';
                if (glare) {
                    glare.style.transition = 'none';
                }
            });
        });
    }

    // 3. Smooth 3D Page Exit Transition on Link Navigation
    function initPageTransitions() {
        document.addEventListener('click', (e) => {
            const link = e.target.closest('a');
            if (!link) return;

            const href = link.getAttribute('href');
            const target = link.getAttribute('target');

            // Skip non-navigational links
            if (!href || href.startsWith('#') || href.startsWith('javascript:') || href.startsWith('mailto:') || href.startsWith('tel:') || target === '_blank') return;
            if (link.hostname && link.hostname !== window.location.hostname) return;
            if (e.ctrlKey || e.metaKey || e.shiftKey) return;

            const main = document.querySelector('main.tagr-main-container');
            if (main) {
                e.preventDefault();
                main.style.transition = 'opacity 0.22s cubic-bezier(0.16, 1, 0.3, 1), transform 0.22s cubic-bezier(0.16, 1, 0.3, 1)';
                main.style.opacity = '0';
                main.style.transform = 'perspective(1200px) rotateX(-4deg) translateY(-14px) scale(0.98)';
                setTimeout(() => {
                    window.location.href = href;
                }, 180);
            }
        });
    }

    // 4. Scroll-Driven 3D Animations Engine (60 FPS Hardware-Accelerated)
    function initScroll3D() {
        // A. Hero Elements 3D Morphing on Scroll
        const heroWrapper = document.getElementById('heroWrapper');
        const heroCard = document.getElementById('heroScrollCard');
        const chip1 = document.getElementById('heroChip1');
        const chip2 = document.getElementById('heroChip2');
        const orb1 = document.getElementById('heroOrb1');
        const orb2 = document.getElementById('heroOrb2');
        const heroContent = document.getElementById('heroContent');

        let isHeroHovered = false;
        if (heroCard) {
            heroCard.addEventListener('mouseenter', () => {
                isHeroHovered = true;
            });
            heroCard.addEventListener('mouseleave', () => {
                isHeroHovered = false;
                heroCard.style.transition = 'transform 0.45s cubic-bezier(0.16, 1, 0.3, 1)';
            });
        }

        let ticking = false;

        function updateHero3D() {
            if (heroWrapper) {
                const scrollY = window.pageYOffset || document.documentElement.scrollTop;
                const heroHeight = heroWrapper.offsetHeight || 650;

                // Animate while hero is within view
                if (scrollY <= heroHeight * 1.5) {
                    const progress = Math.min(Math.max(scrollY / heroHeight, 0), 1);

                    // Morph Hero Card in 3D perspective space (if not currently mouse tilted)
                    if (heroCard && !isHeroHovered) {
                        const tiltX = (progress * 15).toFixed(2);
                        const tiltY = (-progress * 12).toFixed(2);
                        const transZ = (-progress * 48).toFixed(1);
                        const transY = (scrollY * 0.16).toFixed(1);
                        const scale = (1 - progress * 0.07).toFixed(3);

                        heroCard.style.transform = `perspective(1200px) rotateX(${tiltX}deg) rotateY(${tiltY}deg) translateZ(${transZ}px) translateY(${transY}px) scale3d(${scale}, ${scale}, 1)`;
                    }

                    // Morph Satellite Chips with Parallax 3D Depth
                    if (chip1) {
                        const c1Y = (-progress * 28 + scrollY * 0.12).toFixed(1);
                        const c1Z = (progress * 35).toFixed(1);
                        const c1Scale = (1 - progress * 0.08).toFixed(3);
                        chip1.style.transform = `perspective(1200px) translate3d(${-progress * 30}px, ${c1Y}px, ${c1Z}px) scale3d(${c1Scale}, ${c1Scale}, 1)`;
                    }

                    if (chip2) {
                        const c2Y = (progress * 24 + scrollY * 0.2).toFixed(1);
                        const c2Z = (progress * 30).toFixed(1);
                        const c2Scale = (1 - progress * 0.08).toFixed(3);
                        chip2.style.transform = `perspective(1200px) translate3d(${progress * 35}px, ${c2Y}px, ${c2Z}px) scale3d(${c2Scale}, ${c2Scale}, 1)`;
                    }

                    // Morph Glowing Ambient Orbs
                    if (orb1) {
                        orb1.style.transform = `translate3d(${progress * 50}px, ${scrollY * 0.28}px, 0) scale(${1 + progress * 0.25})`;
                    }
                    if (orb2) {
                        orb2.style.transform = `translate3d(${-progress * 40}px, ${scrollY * 0.18}px, 0) scale(${1 - progress * 0.15})`;
                    }

                    // Hero Content subtle parallax depth & fade
                    if (heroContent) {
                        heroContent.style.transform = `perspective(1200px) translateY(${(scrollY * 0.12).toFixed(1)}px)`;
                        heroContent.style.opacity = Math.max(1 - progress * 1.15, 0).toFixed(2);
                    }
                }
            }
            ticking = false;
        }

        function onScroll() {
            if (!ticking) {
                window.requestAnimationFrame(updateHero3D);
                ticking = true;
            }
        }

        if (heroWrapper) {
            window.addEventListener('scroll', onScroll, { passive: true });
            updateHero3D();
        }

        // B. Below Sections 3D Cascade Entry (IntersectionObserver for 60 FPS)
        const scrollSections = document.querySelectorAll('[data-scroll-3d]');
        if ('IntersectionObserver' in window) {
            const observerOptions = {
                root: null,
                rootMargin: '0px 0px -40px 0px',
                threshold: 0.08
            };

            const sectionObserver = new IntersectionObserver((entries, observer) => {
                entries.forEach(entry => {
                    if (entry.isIntersecting) {
                        entry.target.classList.add('is-visible');
                        observer.unobserve(entry.target);
                    }
                });
            }, observerOptions);

            scrollSections.forEach(section => {
                sectionObserver.observe(section);
            });
        } else {
            scrollSections.forEach(section => section.classList.add('is-visible'));
        }
    }

    // 5. Document Ready Setup
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', () => {
            initTheme();
            init3DTilt();
            initPageTransitions();
            initScroll3D();
        });
    } else {
        initTheme();
        init3DTilt();
        initPageTransitions();
        initScroll3D();
    }

})();
